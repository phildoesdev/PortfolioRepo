class_name EnemyDefinitions
extends Node

enum ENEMIES {
    NONE=0,
    HUMANFEMALE=1,
    HUMANMALE=2,
    ZOMBIE=3,
    SKELETON=4,
    }
enum ENEMY_ANIMATION_CATEGORY {
    NONE=0,
    DEFAULT=1
}
enum ENEMY_MVMT_TYPE {
    NONE=0,
    GROUND=1,
    AIR=2
}

var ENEMY_DEFINITIONS_RAW: Dictionary = {}
var ENEMY_DEFINITIONS: Dictionary = {}
const ENEMY_SPRITE_ROOT_PATH:String = "res://src/Scenes/Enemies/EnemySpriteScenes/"
const ENEMY_SPRITE_ROOT_PATH_SUFFIX:String = ".tscn"

func _init() -> void:
    define_enemy_sheets()
    compile_enemy_sheets()

func compile_enemy_sheets() -> void:
    for _i in ENEMIES:
        if ENEMIES[_i] in ENEMY_DEFINITIONS_RAW.keys():
            ENEMY_DEFINITIONS[ENEMIES[_i]] = EnemyFactSheet.new(ENEMY_DEFINITIONS_RAW[ENEMIES[_i]])
    
func define_enemy_sheets() -> void:
    ENEMY_DEFINITIONS_RAW[ENEMIES["ZOMBIE"]] = {
        "id": ENEMIES.ZOMBIE,
        "name_friendly": "Zombie",
        "sprite_sheet_path": "res://src/Assets/SpriteSheets/Enemies/Zombie/Zombie.png",
        "animation_category": ENEMY_ANIMATION_CATEGORY.DEFAULT,
        "health": 75,
        "health_regen": 0.0,
        "walk_speed": 0.2,
        "animation_speed": 1.0,
        "dmg_type_vuln": {Tower.DMG_TYPES.FIRE: 25.0, Tower.DMG_TYPES.ENERGY: 10.0},
        "dmg_type_resists": {Tower.DMG_TYPES.PHYSICAL: 50.0, Tower.DMG_TYPES.COLD: 25.0},
        "base_dmg_output": 10.0,
        "gold_output_range": [100,250],
        "xp_output": 25.0,
        "loot_lvl": 5.0,
    }
    ENEMY_DEFINITIONS_RAW[ENEMIES["SKELETON"]] = {
        "id": ENEMIES.SKELETON,
        "name_friendly": "Skeleton",
        "sprite_sheet_path": "res://src/Assets/SpriteSheets/Enemies/Skeleton/SkeletonBMPs.png",
        "animation_category": ENEMY_ANIMATION_CATEGORY.DEFAULT,
        "health": 25,
        "health_regen": 0.0,
        "walk_speed": 0.5,
        "animation_speed": 1.2,
        "dmg_type_vuln": {Tower.DMG_TYPES.FIRE: 7.5, Tower.DMG_TYPES.ENERGY: 10.0},
        "dmg_type_resists": {Tower.DMG_TYPES.PHYSICAL: 50.0, Tower.DMG_TYPES.COLD: 25.0},
        "base_dmg_output": 10.0,
        "gold_output_range": [50,100],
        "xp_output": 25.0,
        "loot_lvl": 5.0,
    }

func get_factsheet(enemy_id: ENEMIES) -> EnemyFactSheet:
    if not enemy_id in ENEMY_DEFINITIONS.keys():
        prints("No fact sheet exists for this monster:", enemy_id)
        assert(1==0)
    return ENEMY_DEFINITIONS[enemy_id]
    
