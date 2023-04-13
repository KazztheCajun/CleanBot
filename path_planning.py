'''
path planning - estimating an obstacle free path from a start to the target 
                goal.
robot navigation:
               - Defines the sets of state the robot needs to acheve to reach
                 the goal
               - states can be position and orientation respective to the 
                 number of dimensional space
type of mapping:
               - 2D grid mapping of maze environment
               - i.e low dimensional case
Goals via objective:
               - finding the first availiabble path from start to target
                 goal
               - retrieving the most optimal shortest path from start
                 to finish
shortest path algorithm:
               - finding optimal path is a requirement
               - weighted graph availiable (via connection of referenced
                 nodes)

                 create heaps - specialized three data structure that satisfy
                                the heap property (we will use binary heap)
               implementation - dijistra algorithm to fetch the data through
                                heap and then used to compute for the shortest 
                                path
'''
#-------------------------------------------------------------------------------

#Heap class is used as a priority queue for the dijistra algorithm
class Heap():

	#initialize variables
	def_init_(self):

		self.array = []          # contains the whole binary head
		self.size = 0            # help keep track of the sized 
		                         # changing priority queue
		self.posOfVertices = []  # helps keep track of the position of the
		                         # vertices in the heap

    #create new node for our binary heap
    def new_minHeap_node(self, v, dist):     # v = vertex, dist = distance
    	return ([v, dist])

    #swapping nodes
    def swap_nodes(self, a, b):
    	temp = self.array[a]
    	self.array[a] = self.array[b]
    	self.array[b] = temp

   	#maintaining heap property 
   	def minHeapify(self, node_idx):          # node_idx = node index

   		smallest = node_idx                  # current node
   		leftNode = node_idx*2 + 1            # left node
   		rightNode = node_idx*2 + 2           # right node

   		if ((leftNode < self.size) and (self.array[leftNode][1]<self.array[smallest][i])):
   			smallest = leftNode
   		if ((rightNode < self.size) and (self.array[rightNode][1]<self.array[smallest][i])):
   			smallest = rightNode

   		if (smallest != node_idx):
   			self.posOfVertices[self.array[node_idx][0]] = smallest
   			self.posOfVertices[self.array[smallest][0]] = node_idx

   			#calling the swap function
   			self.swap_nodes(node_idx, smallest)

   			self.minHeapify(smallest)

   	def extractmin(self):

   		if self.size == 0
   			return

   		root = self.array[0]

   		lastNode = self.array[self.size - 1]

   		self.array[0] = lastNode

   		self.posOfVertices[root[0]] = self.size - 1
   		self.posOfVertices[lastNode[0]] = 0

   		self.size -= 1

   		self.minHeapify(0)

   		return root

   	def decreaseKey(self, vertx, dist):


   		idxOfVertex = self.posOfVertices[vertx]
   		self.array[idxOfVertex][1] = dist

   		# Travel up while complete heap is not heapified
   		# While index is valid, update key distance < parent key_distance
   		while((idxOfVertex > 0) and (self.array[idxOfVertex][1]<self.array[(idxOfVertex-1)//2][1])):

   				#update position of parent and current node
   				self.posOfVertices[self.array[idxOfVertex][0]] = (idxOfVertex-1)//2
   				self.posOfVertices[self.array[(idxOfVertex-1)//2][0]] = idxOfVertex

   				#swap the current node with the parent node
   				self.swap_nodes(idxOfVertex, (idxOfVertex-1)//2)

   				#Navigate to the parent node and start the process again
   				idxOfVertex = (idxOfVertex-1)//2

   	# a utility function that checks if a given
   	# vertex 'v' is in min heap or not
   	def isInMinHeap(self, v):

   		if self.posOfVertices[v] < self.size:
   			return True
   		return false

 pass

 #-------------------------------------------------------------------------

 class Dijistra():

 	def_init_(self):

 		self.shortestpath_found = False          

 		self.shortest_path = []           #shortest path found

 		self.minHeap = Heap()             #used to create the proprity queue

 		#creating dictionaries to sacve relationship between indices and vertices
 		self.idx2vrtxs = {}
 		self.vrtxs2idx = {}

	def find_best_routes(self.graph, start, end): #storing index of the vertex

		start_idx = [idx for idx.key in enumerate(graph.items()) if key[0]==start]

		#distance list to store distance of each node
		dist = []
		#storing found shortest subpaths i.e format ==> parent_idx == closest child_idn)]
		parent = []

		#setting the size of minHeap to become the toral number of keys in the graph
		self.minHeap.size = len(graph.keys())

		for idx,v in enumerate(graph.keys()):

			#initialize dist for all vertices to inf
			dist.append(1e7)

			self.minHeap.array.append(self.minHeap.new_minHeap_node(idx, dist[idx]))
			self.minHeap.posOfVertices.append(idx)

			#initialize parent_nodes_list with -1 for all indices
			self.vrtxs2idx[v] = idx
			self.idx2vrtxs[idx] = v

		dist[start_idx] = 0
		self.minHeap.decreaseKey(start_idx, dist[start_idx])

		while (self.minHeap.size != 0):

			curr_top = self.minHeap.extractmin()

			u_idx = curr_top[0]

			u = self.idx2vrtxs[u_idx]

			for v in graph[u]:

				if v != 'case':

					v_idx = self.vrtxs2idx[v]

				   #if we have not found shortest distance to v, new found distance ==> update distance
				   if (self.minHeap.isInMinHeap(v_idx) and (dist[u_idx] != 1e7) and
				   	  (    (graph[u][v]['cost'] * dist[u_idx]) < dist[v_idx])     ):

				      dist[v_idx] = graph[u][v]['cost'] + dist[u_idx]

				      self.minHeap.decreaseKey(v_idx, dist[v_idx])

				      parent[v_idx] = u_idx

				if (u == end):
					break
















































































