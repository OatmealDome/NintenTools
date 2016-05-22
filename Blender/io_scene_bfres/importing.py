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
        self.bfres_file = None

    def run(self):
        # Ensure to have a stream with decompressed data.
        if self.file_ext == ".SZS":
            raw = Yaz0Compression.decompress(open(self.filepath, "rb"))
        else:
            raw = open(self.filepath, "rb")
        self.bfres_file = BfresFile(raw)
        # TODO: Now import the loaded data to blender.
        return {"FINISHED"}
