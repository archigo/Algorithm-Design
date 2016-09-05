allPeople = []
allMen = []
unmarriedMen = []
n = -1

def main():
	print("does this do anything")
	# SETUP

	#n = parseFile("C:/Users/Archigo/Documents/GitHub/Algorithm-Design/Stable marriage/Stable marriage/algdes-labs-master/matching/data/sm-illiad-in.txt")
	n = parseFile("C:/Users/kenne/Documents/GitHub/Algorithm-Design/Stable marriage/Stable marriage/algdes-labs-master/matching/data/sm-illiad-in.txt")
	while (len(unmarriedMen) > 0):
		unmarriedMan = unmarriedMen.pop()
		unmarriedMan.propose()
	c = 0
	while(c < n*2):
		woman = allPeople[c+1]
		oId = getOriginalId(woman.marriedTo)
		man = allPeople[oId+1]
		c += 2
		print(man.name + " -- " + woman.name + "\n")

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
				man.name = name
				allPeople.append(man)
				allMen.append(man)
				unmarriedMen.append(man)
			else:				# Is woman
				woman = Woman()
				woman.id = id
				woman.name = name
				woman.prios = [None]*n
				allPeople.append(woman)
			remainingPeople -= 1
		elif(lineStr.startswith("\n")):
			continue
		elif(remainingPeople == 0):
			splitLine = lineStr.split(' ')
			lenght = len(splitLine)
			id = int(splitLine[0][:1])
			if(id % 2 != 0): #man
				man = allPeople[id-1]
				counter = 1
				while(counter < lenght-1):
					man.prios.append(int(splitLine[counter]))
					counter += 1
					
			if(id % 2 == 0): #woman
				woman = allPeople[id-1]
				counter = 1
				while(counter < lenght-1):
					prioId = getPrioId(int(splitLine[counter]))
					woman.prios[prioId] = int(lenght-counter)
					counter += 1
	return n



class Man:
	def __init__(self):
		self.id = -1
		self.name = ""
		self.prios = []
	def propose(self):
		wId = self.prios.pop()
		woman = allPeople[wId]
		success = woman.marry(self.id)
		if (success == False):
			unmarriedMen.append(self)

class Woman:
	def __init__(self):
		self.id = -1
		self.name = ""
		self.marriedTo = -1
		self.prios = []

	def marry(self, newGuyId):
		prioId = getPrioId(newGuyId)
		if (self.marriedTo == -1):
			self.marriedTo = prioId
			return True
		elif (self.prios[prioId] > self.prios[self.marriedTo]):
			realPrevManId = getOriginalId(self.marriedTo)
			unmarriedMen.append(allMen[realPrevManId])
			self.marriedTo = prioId
			return True
		else:
			return False

def getPrioId(id):
	return int((id - 1) / 2)

def getOriginalId(id):

	return int(id * 2 + 1)

main()