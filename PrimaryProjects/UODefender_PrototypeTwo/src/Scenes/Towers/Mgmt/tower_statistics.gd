class_name TowerStatistics
extends Node
"""
A nice container to centralize all tower related statistics and querying of those statistics!

    .. There is a lot to come back and do here
"""
"""
'Tower stats' is almost certianly going to be its own class. Getting working on here, and then will abstract it out
stats todo
    DPS (dmg per sec)
    DPM (min)
    DPH (hr)
    dmg by element
"""
var total_enemies_seen: int = 0     # the number of possible targets that this tower has registered
var total_attacks_out: int = 0            # the number of attacks executed by this tower

var total_dmg_raw_out: float = 0.0      # Dmg before resists and such. Indication of how well a tower might be performing
var total_dmg_actual_out: float = 0.0   # How much health was actually taken off the monster?
var total_kills: float = 0.0        # How many deaths does this tower have tags on?

var kill_book: KillBook

func _init() -> void:
    kill_book = KillBook.new()

func register_kill(death_sheet: Dictionary) -> void:
    """
    We are notified by the enemies we kill that they are dead and we try to 
        put together w/e interesting stats that might be able to come from that
    """
    total_kills += 1
    kill_book.register_kill(death_sheet)
    GameStatisticsSingleton.register_kill(death_sheet)

func register_dmg_raw_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg that the tower 'sends out', before any resists, dmg buffs, or debuffs
    """
    total_dmg_raw_out += dmg_amt
    kill_book.register_dmg_raw_out(fact_sheet_id, dmg_amt)
    GameStatisticsSingleton.register_dmg_raw_out(fact_sheet_id, dmg_amt)
    
func register_dmg_actual_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg done as reported by the enemies who took the dmg
    """
    total_dmg_actual_out += dmg_amt
    kill_book.register_dmg_actual_out(fact_sheet_id, dmg_amt)
    GameStatisticsSingleton.register_dmg_actual_out(fact_sheet_id, dmg_amt)

func register_enemy_seen() -> void:
    """
    We record how many enemies this tower has seen to give the other stats a point of reference. 
    A good build on a tower placed in a poor position may never thrive even though it has much potential
    """
    total_enemies_seen += 1

func register_attack_out() -> void:
    """
    We record when the initial projectile/attack goes out, does not increment if a condition spreads
        to another target, doesn't consider if the target doesnt actually take dmg, etc
    """
    total_attacks_out += 1
    GameStatisticsSingleton.register_attack_out()
