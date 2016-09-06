allPeople = []
allMen = []
unmarriedMen = []
n = -1

def main():
	# SETUP

	#n = parseFile( "algdes-labs-master/matching/data/sm-illiad-in.txt")
	n = parseFile( "algdes-labs-master/matching/data/sm-bbt-in.txt")
	while (len(unmarriedMen) > 0):
		unmarriedMan = unmarriedMen.pop(0)
		unmarriedMan.propose()
	c = 0
	while(c < n*2):

		man = allPeople[c];
		woman = allPeople[man.marriedTo - 1]

		##woman = allPeople[c+1]
		##oId = getOriginalId(woman.marriedTo)
		##man = allPeople[oId+1]
		c += 2
		print(man.name + " -- " + woman.name)

def parseFile(path):
	file = open(path, "r")
	remainingPeople = -1
	for line in file:
		lineStr = str(line)
		if (lineStr.startswith('#')):
			continue
		elif (lineStr.startswith('n')):
			numberStr = lineStr[2:]
			n = int(numberStr)
			remainingPeople = n*2
		elif (remainingPeople > 0):
			idAndName = lineStr.split(' ')
			id = int(idAndName[0])
			name = str(idAndName[1])
			if (id % 2 != 0):	# Is man
				man = Man()
				man.id = id
				man.name = name[:-1]
				allPeople.append(man)
				allMen.append(man)
				unmarriedMen.append(man)
			else:				# Is woman
				woman = Woman()
				woman.id = id
				woman.name = name[:-1]
				woman.prios = [None]*n
				allPeople.append(woman)
			remainingPeople -= 1
		elif(lineStr.startswith("\n")):
			continue
		elif(remainingPeople == 0):
			splitLine = lineStr.split(' ')
			length = len(splitLine) - 1
			id = int(splitLine[0][:-1])
			if(id == 5 | id == 88):
				debug = True;

			if(id % 2 != 0): #man
				man = allPeople[id-1]
				counter = 1
				while(counter < length):
					man.prios.append(int(splitLine[counter]))
					counter += 1

			if(id % 2 == 0): #woman
				woman = allPeople[id-1]
				counter = 1
				while(counter < length):
					prioId = getPrioId(int(splitLine[counter]))
					woman.prios[prioId] = int(length-counter)
					counter += 1
	return n



class Man:
	def __init__(self):
		self.id = -1
		self.name = ""
		#prio stack, value at each index is the priority of the person with that index e.g. 
		#prios[10] = 70 means this person has given person with index 10 prio 70
		self.prios = []
		self.marriedTo = -1

	def propose(self):

		if(self.id == 5):
			debug = True;

		wId = self.prios.pop(0)
		woman = allPeople[wId-1]
		success = woman.marry(self.id)
		if (success == False):
			unmarriedMen.append(self)
		else:
			self.marriedTo = woman.id

class Woman:
	def __init__(self):
		self.id = -1
		self.name = ""
		self.marriedTo = -1
		self.prios = [] #contains ids transformed using getPrioId(int)

	def marry(self, newGuyId):

		if(self.id == 6):
			debug = True;

		prioId = getPrioId(newGuyId)
		if (self.marriedTo == -1):
			self.marriedTo = prioId
			return True
		elif (self.prios[prioId] > self.prios[self.marriedTo]):
			realPrevManId = getOriginalId(self.marriedTo)
			unmarriedMen.append(allPeople[realPrevManId - 1])
			self.marriedTo = getOriginalId(prioId)
			return True
		else:
			return False

def getPrioId(id):
	return int((id - 1) / 2)

def getOriginalId(id):

	return int(id * 2 + 1)

main()