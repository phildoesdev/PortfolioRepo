class_name GameStatistics
extends Node

"""
var total_enemies_seen: int = 0     # the number of possible targets that this tower has registered
var attacks_out: int = 0            # the number of attacks executed by this tower
var dmg_out_raw: float = 0.0        # the amt of dmg the tower would put out before resists, armor, etc.
var dmg_out_actual: float = 0.0     # the amt of dmg dealth (actual hit points subtracked from monsters

var total_dmg_raw_out: float = 0.0      # Dmg before resists and such. Indication of how well a tower might be performing
var total_dmg_actual_out: float = 0.0   # How much health was actually taken off the monster?

var kill_book: KillBook
var total_kills: float = 0.0        # How many deaths does this tower have tags on?
"""
var gamestats_groups: Array = ["GameStatistics"]   # How most other things will reference us 

# Things that we'll likely care about at a global lvl
var total_enemies_spawned: int = 0
var total_towers_placed: int = 0
var total_towers_dismissed: int = 0

## Duplicate vars from tower stats that will be interesting on a global vll
var total_enemies_seen: int = 0
var total_attacks_out: int = 0
var total_dmg_raw_out: float = 0.0
var total_dmg_actual_out: float = 0.0
var total_kills: int = 0.0
var master_killbook: KillBook

func _init() -> void:
    master_killbook = KillBook.new()
    for _group in gamestats_groups:
        add_to_group(_group)

func update_total_enemies_spawned(amt:int = 1) -> void:
    """
    Method to update total_enemies_spawned statistics
    """
    total_enemies_spawned += amt

func update_total_towers_placed(amt:int = 1) -> void:
    """
    Method to update total_towers_spawned statistics
    """
    total_towers_placed += amt

func update_total_towers_dismissed(amt:int = 1) -> void:
    """
    Method to update total_towers_dismissed statistics
    """
    total_towers_dismissed += amt

func register_kill(death_sheet: Dictionary) -> void:
    """
    We are notified by the enemies we kill that they are dead and we try to 
        put together w/e interesting stats that might be able to come from that
    """
    total_kills += 1
    master_killbook.register_kill(death_sheet)

func register_dmg_raw_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg that the tower 'sends out', before any resists, dmg buffs, or debuffs
    """
    total_dmg_raw_out += dmg_amt
    master_killbook.register_dmg_raw_out(fact_sheet_id, dmg_amt)
    
func register_dmg_actual_out(fact_sheet_id:int, dmg_amt:float) -> void:
    """
    This is the dmg done as reported by the enemies who took the dmg
    """
    total_dmg_actual_out += dmg_amt
    master_killbook.register_dmg_actual_out(fact_sheet_id, dmg_amt)

func register_attack_out() -> void:
    """
    Registers how many attacks out our towers produced
    """
    total_attacks_out += 1
