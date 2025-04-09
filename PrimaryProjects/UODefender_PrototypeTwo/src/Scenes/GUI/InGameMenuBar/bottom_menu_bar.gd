class_name BottomMenuBarControl
extends Control
"""
For now we're just going to have a bunch of labels to update... some stuff internally, some stuff
    people are going to have to ask for and call
"""

var bmbc_groups: Array = ["BottomMenuBarControl","GoldDisplay"]

## DEBUG
@onready var enemies_spawned: Label = $Debug/EnemiesSpawned/EnemiesSpawned
@onready var enemies_killed: Label = $Debug/EnemiesKilled/EnemiesKilled
@onready var elapsed_time: Label = $Debug/ElapsedTime/ElapsedTime

## Reg
@onready var gate_health: Label = $Health/GateHealth
@onready var gold_amt: Label = $Gold/GoldAmt

var player_bank_group_name: String = "PlayerBank"   # The group name where we can expect balance changed signal from

func _ready() -> void:
    for _group in bmbc_groups:
        add_to_group(_group)
    register_gp_balance_listener()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(_delta: float) -> void:
    update_time_elapsed()
    update_from_game_stats_singleton()

func register_gp_balance_listener() -> void:
    """
    Goes through and looks for specific listeners and trys to set them up
        for things like 'find player and connect the signal that notifies us about gp balnace
        changes'
    """
    # Making this vague b/c if we refact all we are expecting is this signal name and method call avail
    var _source: Node = get_tree().get_first_node_in_group(player_bank_group_name)
    assert(_source.has_method("balance_gp") and _source.has_signal("balance_changed"), "Invalid gp balance source. WMH3D78Y")    
    update_gold_display(_source.balance_gp())
    _source.balance_changed.connect(update_gold_display)

func update_time_elapsed() -> void:
    """
    Keep trakc of the time
    """
    elapsed_time.text = str(floor(Time.get_ticks_msec()/1000))

func update_from_game_stats_singleton() -> void:
    """
    Based on the game stats singleton, update some thigns on the screen, mostly debug
    """
    enemies_spawned.text = str(GameStatisticsSingleton.total_enemies_spawned)
    enemies_killed.text = str(GameStatisticsSingleton.total_kills)

func update_gold_display(gp: int) -> void:
    gold_amt.text = str(gp)
