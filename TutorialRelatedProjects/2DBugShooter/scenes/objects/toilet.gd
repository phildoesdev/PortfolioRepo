extends ItemContainer



# Called when the node enters the scene tree for the first time.
func _ready():
	item_name = "Toilet"


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass

func hit(modifier=3):
	super(modifier)
