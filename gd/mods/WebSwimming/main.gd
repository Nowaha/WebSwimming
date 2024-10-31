extends Node

var debug = false

var map: Node = null
var queue = []

func give_water_collision(node: Node, small: bool) -> void:
	var size = 20 if small else 80
	var y = 2 if small else 3.5
	
	var water_body = StaticBody.new()
	var collision_shape = CollisionShape.new()
	
	var box_shape = BoxShape.new()
	box_shape.extents = Vector3(size, 0.5, size)
	
	collision_shape.shape = box_shape
	
	water_body.translation = Vector3(0, y, 0)
	water_body.add_child(collision_shape)
	node.add_child(water_body)
	
func give_water_collisions_recursively(node: Node) -> void:
	for child in node.get_children():
		if check_if_node_has_script(child, "res://Scenes/Map/Props/extreme_water_small.gd"):
			give_water_collision(child, true)
		elif check_if_node_has_script(child, "res://Scenes/Map/Props/water_main.gd"):
			give_water_collision(child, false)
		else:
			give_water_collisions_recursively(child)

func _on_swim_start():
	var player = find_player()
	if player == null:
		print("Player null!")
		return
	
	player.jump_height = 8.0 # higher than default
	# player.accel = 16.0
	# player.jump_height = 4.0
	# player.dive_distance = 12.0
	player._sync_sfx("drown")
	player._sync_particle("splash", player.global_transform.origin, true)

func _on_swim_stop():
	var player = find_player()
	if player == null:
		print("Player null!")
		return
	
	player.jump_height = 6.0 # default
	# player.accel = 64.0
	# player.dive_distance = 9.0
	player._sync_sfx("splashb")

func _ready():
	get_tree().connect("node_added", self, "_on_node_added")
	
	PlayerData.connect("_swim_start", self, "_on_swim_start")
	PlayerData.connect("_swim_stop", self, "_on_swim_stop")
	
	if debug:
		setup_q()
		
func _on_node_added(added_node):
	if added_node.name == "main_map":
		map = added_node
		
		var zones: Node = map.get_child(3)
		var main_zone: Node = zones.get_child(0)
		give_water_collisions_recursively(main_zone)




# Utils
func find_player():
	var nodes = get_tree().get_nodes_in_group("player")
	
	if nodes.empty(): return null
	return nodes[0]
	
func check_if_node_has_script(node: Node, scriptName: String) -> bool:
	var script = node.get_script()
	return script != null and is_instance_valid(script) and script.resource_path == scriptName


# Debug stuff
func setup_q() -> void:
	var timer = Timer.new()
	timer.wait_time = 1.0
	timer.autostart = true
	timer.connect("timeout", self, "handle_q")
	add_child(timer)

func q(message: String) -> void:
	queue.append(message)
	
func handle_q() -> void:
	print(queue)
	queue.clear()

func qn(node: Node, level: int = 0, max_level: int = 999) -> void:
	print(" ".repeat(level) + node.name)
	
	if level >= max_level: return
	for child in node.get_children():
		qn(child, level + 1, max_level)
