import bmesh
import bpy
import bpy_extras
import io
import os
from .yaz0 import Yaz0Compression
from .bfres_file import BfresFile

class ImportOperator(bpy.types.Operator, bpy_extras.io_utils.ImportHelper):
    bl_idname = "import_scene.bfres"
    bl_label = "Import BFRES"
    bl_options = {"UNDO"}

    filename_ext = ".bfres"
    filter_glob = bpy.props.StringProperty(
        default="*.bfres;*.szs",
        options={"HIDDEN"}
    )
    filepath = bpy.props.StringProperty(
        name="File Path",
        description="Filepath used for importing the BFRES or compressed SZS file",
        maxlen=1024,
        default=""
    )
    merge_seams = bpy.props.BoolProperty(
        name="Merge Seam Vertices",
        description="Remerge vertices which were split to create UV seams.",
        default=True
    )

    def execute(self, context):
        from . import importing
        importer = importing.Importer(self, context, self.properties.filepath)
        return importer.run()

    @staticmethod
    def menu_func_import(self, context):
        self.layout.operator(ImportOperator.bl_idname, text="Nintendo BFRES (.bfres/.szs)")

class Importer:
    def __init__(self, operator, context, filepath):
        self.operator = operator
        self.context = context
        # Extract path information.
        self.filepath = filepath
        self.directory = os.path.dirname(self.filepath)
        self.filename = os.path.basename(self.filepath)
        self.fileext = os.path.splitext(self.filename)[1].upper()
        # Create work directories for temporary files.
        self.work_directory = os.path.join(self.directory, self.filename + ".work")
        os.makedirs(self.work_directory, exist_ok=True)
        self.gfx2_directory = os.path.join(self.work_directory, "gfx2")
        os.makedirs(self.gfx2_directory, exist_ok=True)

    def run(self):
        # Ensure to have a stream with decompressed data.
        if self.fileext == ".SZS":
            raw = io.BytesIO(Yaz0Compression.decompress(open(self.filepath, "rb")))
        else:
            raw = open(self.filepath, "rb")
        bfres_file = BfresFile(raw)
        raw.close()
        # Import the data into Blender objects.
        self._convert_bfres(bfres_file)
        return {"FINISHED"}

    def _convert_bfres(self, bfres):
        # Go through the FMDL sections which map to a Blender object.
        for fmdl_node in bfres.fmdl_index_group[1:]:
            self._convert_fmdl(bfres, fmdl_node.data)

    def _convert_fmdl(self, bfres, fmdl):
        # Create an object for this FMDL in the current scene.
        fmdl_obj = bpy.data.objects.new(fmdl.header.file_name_offset.name, None)
        bpy.context.scene.objects.link(fmdl_obj)
        # Go through the polygons in this model.
        for fshp_node in fmdl.fshp_index_group[1:]:
            self._convert_fshp(bfres, fmdl, fmdl_obj, fshp_node.data)

    def _convert_fshp(self, bfres, fmdl, fmdl_obj, fshp):
        # Get the vertices and indices of the most detailled LoD model.
        lod_model = fshp.lod_models[0]
        vertices = fmdl.fvtx_array[fshp.header.buffer_index].get_vertices()
        indices = lod_model.get_indices_for_visibility_group(0)
        # Create a bmesh to represent the FSHP polygon.
        bm = bmesh.new()
        # Go through the vertices (starting at the given offset) and add them to the bmesh.
        # This would also add the vertices of all other LoD models. As there is no direct way to get the number of
        # vertices required for the current LoD model (the game does not need that), get the last indexed one with max.
        last_vertex = max(indices) + 1
        for vertex in vertices[lod_model.skip_vertices:lod_model.skip_vertices + last_vertex]:
            bm_vert = bm.verts.new((vertex.p0[0], vertex.p0[2], vertex.p0[1])) # Exchange Y with Z
            #bm_vert.normal = vertex.n0 # Blender does not correctly support custom normals, and they look weird.
        bm.verts.ensure_lookup_table()
        bm.verts.index_update()
        # Connect the faces (they are organized as a triangle list) and smooth shade them.
        for i in range(0, len(indices), 3):
            face = bm.faces.new(bm.verts[j] for j in indices[i:i + 3])
            face.smooth = True
        # Set the UV coordinates by iterating through the face loops and getting their vertex' index.
        uv_layer = bm.loops.layers.uv.new()
        for face in bm.faces:
            for loop in face.loops:
                uv = vertices[loop.vert.index + lod_model.skip_vertices].u0
                loop[uv_layer].uv = (uv[0], 1 - uv[1]) # Flip Y
        # Optimize the mesh if requested.
        if self.operator.merge_seams:
            bmesh.ops.remove_doubles(bm, verts=bm.verts, dist=0)
        # Write the bmesh data back to a new mesh.
        fshp_mesh = bpy.data.meshes.new(fshp.header.name_offset.name)
        bm.to_mesh(fshp_mesh)
        bm.free()
        # Apply the referenced material to the mesh.
        fmat = fmdl.fmat_index_group[fshp.header.material_index + 1].data
        fshp_mesh.materials.append(self._get_fmat_material(bfres, fmat))
        # Create an object to represent the mesh with.
        fshp_obj = bpy.data.objects.new(fshp_mesh.name, fshp_mesh)
        fshp_obj.parent = fmdl_obj
        bpy.context.scene.objects.link(fshp_obj)

    def _get_fmat_material(self, bfres, fmat):
        # Return a previously created material or make a new one.
        material_name = fmat.header.name_offset.name
        material = bpy.data.materials.get(material_name)
        if not material is None:
            return material
        material = bpy.data.materials.new(material_name)
        # Convert and load the textures into the materials' texture slots.
        # textures: fmat.texture_selector_array (offset and name like "Iggy_Alb")
        # texture influences: fmat.texture_attribute_selector_index_group -> attribute_name_offset.name (_a0, _n0, ...)
        for tex_selector, attrib in zip(fmat.texture_selector_array, fmat.texture_attribute_selector_index_group[1:]):
            if self._check_texture_attrib_supported(attrib):
                slot = material.texture_slots.add()
                self._configure_texture_slot(slot, bfres, tex_selector, attrib)
        return material

    def _check_texture_attrib_supported(self, attrib):
        attrib_name = attrib.name_offset.name
        # Bake textures are not supported at the moment.
        return attrib_name[1] != "b"

    def _configure_texture_slot(self, slot, bfres, texture_selector, texture_attrib):
        slot.texture = self._get_ftex_texture(bfres, texture_selector)

    def _get_ftex_texture(self, bfres, texture_selector):
        # Return a previously created texture or make a new one.
        texture_name = texture_selector.name_offset.name
        texture = bpy.data.textures.get(texture_name)
        if not texture is None:
            return texture
        # Export the FTEX section referenced by the texture selector as a GFX2 file.
        ftex = bfres.ftex_index_group[texture_selector.ftex_offset].data
        gfx2_filename = os.path.join(self.gfx2_directory, ftex.header.file_name_offset.name + ".gfx2")
        ftex.export_gtx(open(gfx2_filename, "wb"))
        # TODO: Delegate the conversion to TexConv2.
        # Load the converted DDS texture.
        texture = bpy.data.textures.new(texture_name, "IMAGE")
        # TODO: texture.image = ?
