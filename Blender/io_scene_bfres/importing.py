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
        self.filepath = filepath
        self.file_ext = os.path.splitext(self.filepath)[1].upper()
        self.directory = os.path.dirname(self.filepath)

    def run(self):
        # Ensure to have a stream with decompressed data.
        if self.file_ext == ".SZS":
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
        obj = bpy.data.objects.new(fmdl.header.file_name_offset.name, None)
        bpy.context.scene.objects.link(obj)
        # Go through the polygons in this model.
        for fshp_node in fmdl.fshp_index_group[1:]:
            self._convert_fshp(bfres, fmdl, obj, fshp_node.data)

    def _convert_fshp(self, bfres, fmdl, fmdl_obj, fshp):
        # Get the indices and vertices of the most detailled LoD model.
        lod_model = fshp.lod_models[0]
        indices = lod_model.get_indices_for_visibility_group(0)
        vertices = fmdl.fvtx_array[fshp.header.buffer_index].get_vertices()
        # Create a mesh and a bmesh of it as the child of the parent object.
        mesh = bpy.data.meshes.new(fshp.header.name_offset.name)
        bm = bmesh.new()
        bm.from_mesh(mesh)
        # Go through the indices and add the referenced vertices to the bmesh.
        for i in indices:
            vertex = vertices[i]
            bm.verts.new(vertex.p0)
        # Connect the faces, as they are organized as a triangle list.
        bm.verts.ensure_lookup_table() # Required since 2.73 before accessing vertices with index.
        for i in range(0, len(indices), 3):
            bm.faces.new((bm.verts[j] for j in range(i, i + 3)))
        # Write the bmesh data back to the mesh.
        bm.to_mesh(mesh)
        bm.free()
        # Create an object to represent the mesh with.
        fshp_obj = bpy.data.objects.new(mesh.name, mesh)
        fshp_obj.parent = fmdl_obj
        bpy.context.scene.objects.link(fshp_obj)
