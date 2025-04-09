"""
******* FOR REBUILD...
    purge all the things that I am not positive that I want, I am going to add more later so extra things seem annoying
    Plan to expand it
    It would be nice to be able to use broad names like 'SPEEDSTER' to apply stats to monsters rather than having to do that myself?? ( I think this is actually stupid, and limiting. i should do this conversion in the spreadsheet, not here.)
    I think that I'll follow this same 'factory' design pattern to build out the different types of enemies I care about, and classify everything with enums?
"""

class_name EnemyFactSheet
extends Node

# Used to keep track of whether our fact sheet initialized correctly. Essentially 'Did someone create this factsheet with all the required fields
var is_fs_viable: bool = false

# Info from our intial enemy fact sheet - if adding here, add to '_to_dict()' method, as this is used to verify incoming raw fact sheet structs
var id:int = 0
var name_friendly:String = ""       # Used to show to players
var sprite_sheet_path:String = ""   # path to the spritesheet for this enemy
var animation_category: EnemyDefinitions.ENEMY_ANIMATION_CATEGORY = EnemyDefinitions.ENEMY_ANIMATION_CATEGORY.NONE  # Defines what animations we need to use for different things such as walking. 'DEFAULT' works for most basic monsters
var animation_speed: float = 0.0
var walk_speed: float = 0.0
var health: float = 0.0
var health_regen: float = 0.0
var dmg_type_vuln:Dictionary = {}
var dmg_type_resists:Dictionary = {}
var attack_speed:float = 0.0
var base_dmg_output:float = 0.0
var gold_output_range:Array = []
var xp_output:float = 0.0
var loot_lvl:float = 0.0

# Calculated values
var gold_output: int = 0  # Calc'd based on the gold_output_range

func _to_string() -> String:
    return str(self._to_dict())

func _to_dict() -> Dictionary:
    # Used for niceness, also used to verify the passed in fact sheet has the correct keys
    return {
        "id": id,
        "name_friendly": name_friendly,
        "sprite_sheet_path": sprite_sheet_path,
        "animation_category": animation_category,
        "health": health,
        "health_regen": health_regen,
        "walk_speed": walk_speed,
        "animation_speed": animation_speed,
        "dmg_type_vuln": dmg_type_vuln,
        "dmg_type_resists": dmg_type_resists,
        "base_dmg_output": base_dmg_output,
        "gold_output_range": gold_output_range,
        "xp_output": xp_output,
        "loot_lvl": loot_lvl,
    }
    
func get_deathsheet() -> Dictionary:
    return {
        "id": id,
        "name_friendly": name_friendly,
        "dmg_output": base_dmg_output,
        "gold_output": gold_output,
        "xp_output": xp_output,
        }

func get_factsheet() -> Dictionary:
    var rtn_val: Dictionary = self._to_dict()
    rtn_val["is_fs_viable"] = is_fs_viable
    return rtn_val

func _init(fact_sheet_array: Dictionary) -> void:
    """
    Passing in a dictionary instead of w/e else because I think that it makes it less readable here initially, but
        much more readable and manageable on the other side (the json struct of enemy info that we build from).
    The only downside imo is that we have to do a verify on the dictionary to make sure it has our params and what have you
    """
    # Set 'is ready' - as in, is this a valid character sheet
    is_fs_viable = _verify_fact_sheet(fact_sheet_array)
    # Allow bypass for empty 
    if not is_fs_viable:
        print("Invalid enemy fact sheet. VNM8M0DW")
        return
    # Pull data from struct and assign it
    id = fact_sheet_array["id"]
    name_friendly = fact_sheet_array["name_friendly"]
    sprite_sheet_path = fact_sheet_array["sprite_sheet_path"]
    animation_category = fact_sheet_array["animation_category"]
    health = fact_sheet_array["health"]
    walk_speed = fact_sheet_array["walk_speed"]
    animation_speed = fact_sheet_array["animation_speed"]
    dmg_type_vuln = fact_sheet_array["dmg_type_vuln"]
    dmg_type_resists = fact_sheet_array["dmg_type_resists"]
    base_dmg_output = fact_sheet_array["base_dmg_output"]
    gold_output_range = fact_sheet_array["gold_output_range"]
    xp_output = fact_sheet_array["xp_output"]
    loot_lvl = fact_sheet_array["loot_lvl"]
    
func _verify_fact_sheet(fact_sheet_array: Dictionary) -> bool:
    """
    Very simple verification on our passed in dictionary. 
    No type checks or anything like this currently, we are just guarenteeing that the required fields are present
    """
    for _k in self._to_dict().keys():
        if _k not in fact_sheet_array.keys():
            print("MISSING KEY: ", _k)
            return false
    return true

func is_viable() -> bool:
    return is_fs_viable
    
func scale_factsheet() -> void:
    """
    Will, someday, allow us to scale a monster up or down smartly w/ minimual work 
    """
    pass

func calculate_gold_output() -> void:
    randomize()
    gold_output = randi_range(gold_output_range[0],gold_output_range[1])
