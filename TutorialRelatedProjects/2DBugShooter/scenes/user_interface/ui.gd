extends CanvasLayer

@onready var laser_label: Label = $Counters/CounterGrid/BulletCounter/VBoxContainer/BulletCounter
@onready var grenade_label: Label = $Counters/CounterGrid/GrenadeCounter/VBoxContainer/GrenadeCounter
@onready var laser_icon: TextureRect = $Counters/CounterGrid/BulletCounter/VBoxContainer/BulletIcon
@onready var grenade_icon: TextureRect = $Counters/CounterGrid/GrenadeCounter/VBoxContainer/GrenateIcon
@onready var health_bar: TextureProgressBar = $HealthBar/HealthBar

var green: Color = Color("6bbfa3")
var red: Color = Color(0.9,0,0,1)

func update_laser_text():
	laser_label.text = str(Globals.laser_amount)
	update_color(Globals.laser_amount, laser_label, laser_icon)
	
func update_grenade_text():
	grenade_label.text = str(Globals.grenade_amount)
	update_color(Globals.grenade_amount, grenade_label, grenade_icon)
	
func update_health_text():
	health_bar.value = Globals.health

func update_color(amt: int, lbl: Label, icon: TextureRect) -> void:
	if amt <= 0:
		lbl.modulate = red
		icon.modulate = red
	else:
		lbl.modulate = green
		icon.modulate = green

func _ready():
	Globals.connect("health_change", update_health_text)
	Globals.connect("laser_change", update_laser_text)
	Globals.connect("grenade_change", update_grenade_text)
	
	update_laser_text()
	update_grenade_text()
	update_health_text()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass
