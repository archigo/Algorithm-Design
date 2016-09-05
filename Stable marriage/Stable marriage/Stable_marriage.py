allPeople = []
allMen = []
unmarriedMen = []
n = -1

def run(self):
	# SETUP
	classmethod()

	parseFile("/algdes-labs-master/matching/data/sm-illiad-in.txt")
	while (unmarriedMen.count > 0):
		unmarriedMan = unmarriedMen.pop()
		unmarriedMan.propose()

def parseFile(self, path):
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
				allPeople.append(woman)
			remainingPeople =- 1
		elif(lineStr.startswith(' ')):
			continue
		elif(remainingPeople == 0):
			splitLine = lineStr.split(' ')
			lenght = len(splitLine)
			id = int(splitLine[0][:1])
			if(id % 2 != 0): #man
				man = Man(allPeople[id-1])
				counter = 1
				while(counter < lenght):
					man.prios.append(int(splitLine[counter]))
			if(id % 2 == 0): #woman
				woman = Woman(allPeople[id-1])
				counter = 1
				while(counter < lenght):
					prioId = getPrioId(int(splitLine[counter]))
					woman.prios[prioId] = int(lenght-counter)



class Man:
	id = -1
	name = ""
	prios = []
	def propose(self):
		wId = prios.pop()
		woman = allPeople[wId-1]
		success = woman.marry(id)
		if (success == False):
			unmarriedMen.append(self)

class Woman:
	id = -1
	name = ""
	marriedTo = -1
	prios = []
	def marry(self, newGuyId):
		prioId = getPrioId(newGuyId)
		if (marriedTo == -1):
			marriedTo = prioId
			return True
		elif (prios[prioId] > prios[marriedTo]):
			realPrevManId = getOriginalId(marriedTo)
			unmarriedMen.append(allMen[realPrevManId])
			marriedTo = prioId
			return True
		else:
			return False

def getPrioId(id):
	return int((id - 1) / 2)

def getOriginalId(id):

	return int(id * 2 + 1)