import bmesh
import bpy
import bpy_extras
import os
from .bfres_file import BfresFile


class ImportOperator(bpy.types.Operator, bpy_extras.io_utils.ImportHelper):
    bl_idname = "import_scene.bfres"
    bl_label = "Import BFRES"
    bl_options = {"UNDO"}

    filename_ext = ".bfres"
    filter_glob = bpy.props.StringProperty(
        default="*.bfres",
        options={"HIDDEN"}
    )
    filepath = bpy.props.StringProperty(
        name="File Path",
        description="Filepath used for importing the BFRES file",
        maxlen=1024,
        default=""
    )

    def execute(self, context):
        from . import importing
        importer = importing.Importer(self, context, self.properties.filepath)
        return importer.run()

    @staticmethod
    def menu_func_import(self, context):
        self.layout.operator(ImportOperator.bl_idname, text="Nintendo BFRES (.bfres)")


class Importer:
    def __init__(self, operator, context, filepath):
        self.operator = operator
        self.context = context
        self.filepath = filepath
        self.directory = os.path.dirname(self.filepath)
        self.bfres_file = None

    def run(self):
        # Load the BFRES file contents.
        self.bfres_file = BfresFile(open(self.filepath, "rb"))
        return {"FINISHED"}

    @staticmethod
    def _log(message):
        print("BFRES: " + message)