extends RigidBody2D

signal explode_signal(modulator)

const speed: int = 1000
const hit_radius: int = 250
const dmg_fire: float = .01
const dmg_explo: float = 2

func explode():
    find_hit(dmg_explo)
    $dmg_timer.start()
    
func find_hit(dmg_ttl):
    for target in get_tree().get_nodes_in_group("Entity"):
        if "hit" in target and position.distance_to(target.position) <= hit_radius:
            target.hit(dmg_ttl*Globals.attack_power)

func _on_dmg_timer_timeout():
    # Instead of doing a timer, he set a bool 'explosion active', and then acted on this within the _process method. 
    # That would probably be better because the amt of dmg this is going to do with a timer is a little abstract. 
    # Doing thigns w/ a bool, in process would mean that we could (maybe?) calculate dmg/second more easily
    find_hit(dmg_fire)
