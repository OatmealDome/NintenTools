bl_info = {
    "name": "Nintendo BFRES format",
    "author": "Ray Koopa",
    "version": (16, 5, 18),
    "blender": (2, 77, 0),
    "location": "File > Import-Export",
    "description": "Import-Export BFRES mesh, UV's, materials and textures",
    "category": "Import-Export",
    "warning": "This add-on is under development."
}

# Reload the classes when reloading add-ons in Blender with F8.
if "bpy" in locals():
    import importlib
    if "binary_io" in locals():
        print("Reloading: " + str(binary_io))
        importlib.reload(binary_io)
    if "bfres_common" in locals():
        print("Reloading: " + str(bfres_common))
        importlib.reload(bfres_common)
    if "bfres_fmdl" in locals():
        print("Reloading: " + str(bfres_fmdl))
        importlib.reload(bfres_fmdl)
    if "bfres_file" in locals():
        print("Reloading: " + str(bfres_file))
        importlib.reload(bfres_file)
    if "importing" in locals():
        print("Reloading: " + str(importing))
        importlib.reload(importing)

import bpy
from . import binary_io
from . import bfres_common
from . import bfres_fmdl
from . import bfres_file
from . import importing

def register():
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_import.append(importing.ImportOperator.menu_func_import)

def unregister():
    bpy.utils.unregister_module(__name__)
    bpy.types.INFO_MT_file_import.remove(importing.ImportOperator.menu_func_import)

# Register classes of the add-on when Blender runs this script.
if __name__ == "__main__":
    register()
