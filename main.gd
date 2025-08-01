extends Node

func _ready():
	ErrorCapture.ErrorCaught.connect(func(err):print(err))	
	var a = 19
	var b = "11"
	# var c = a + b
	# var d = c - b
	var res = ResourceLoader.load("res://114514.tres")
	# print(res.resource_path)

func _process(delta: float) -> void:
	# print("ERROR:")
	pass
