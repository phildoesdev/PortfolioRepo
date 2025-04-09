class_name DebugDraw3D
extends Node3D
"""
Generic draw functinoality to help debug our projects
"""

# Settings
var oneline: bool = true
var default_hue: Color = Color.RED
var default_sphere_rad: float = 0.25

## Line Info
var ln_container: Node3D
var sphere_container: Node3D

func _ready() -> void:
    if not ln_container:
        ln_container = Node3D.new()
        add_child.call_deferred(ln_container)
    if not sphere_container:
        sphere_container = Node3D.new()
        add_child.call_deferred(sphere_container)

func draw_line(from: Vector3, to: Vector3, hue: Color=default_hue) -> void:
    # Sanity check
    assert(ln_container)
    # Boilerplate foundation for our line
    var ln_instance: MeshInstance3D = MeshInstance3D.new()
    var ln_mesh_immediate: ImmediateMesh = ImmediateMesh.new()
    var ln_material: StandardMaterial3D = StandardMaterial3D.new()
    ln_instance.mesh = ln_mesh_immediate
        
    ln_material.albedo_color = hue
    ln_mesh_immediate.surface_begin(Mesh.PRIMITIVE_LINES, ln_material)
    from = to_local(from)
    to = to_local(to)
    ln_mesh_immediate.surface_add_vertex(from)
    ln_mesh_immediate.surface_add_vertex(to)
    ln_mesh_immediate.surface_end()
    ln_container.add_child(ln_instance)
    
func clear_lines() -> void:
    for line in ln_container.get_children():
        if line is MeshInstance3D and line.mesh is ImmediateMesh:
            line.mesh.clear_surfaces()
            line.queue_free()

func draw_sphere(pos: Vector3, rad:float = default_sphere_rad, hue: Color = default_hue):
    # Sanity check
    assert(sphere_container)
    # Setup MeshInstance, it's mesh, and it's material
    var pnt_mesh := MeshInstance3D.new()
    var pnt_mesh_sphere: SphereMesh = SphereMesh.new()
    var pnt_material := StandardMaterial3D.new()
    
    pnt_mesh.mesh = pnt_mesh_sphere
    pnt_mesh_sphere.radius = rad
    pnt_mesh_sphere.height = rad*2
    pnt_mesh_sphere.material = pnt_material
    pnt_material.albedo_color = hue
    sphere_container.add_child(pnt_mesh)
    pnt_mesh.global_position = pos
    
    # Setup collision shape and its shape
    var pnt_collision: CollisionShape3D = CollisionShape3D.new()
    var pnt_collsiion_shape: SphereShape3D = SphereShape3D.new()
    pnt_collsiion_shape.radius = rad*1.2
    pnt_collision.shape = pnt_collsiion_shape
    sphere_container.add_child(pnt_collision)
    pnt_collision.global_position = pos
    
func clear_spheres() -> void:
    for sph in sphere_container.get_children():
        sph.queue_free()

func clear_all() -> void:
    clear_lines()
    clear_spheres()
