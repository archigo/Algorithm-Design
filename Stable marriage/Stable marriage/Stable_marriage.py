allPeople = []
allMen = []
unmarriedMen = []
n = -1

def run(self):
	# SETUP
	classmethod()
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
			remainingPeople = n
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



class Man:
	id = -1
	name = ""
	prios = []
	def propose(self):
		w = prios.pop()
		success = w.marry(id)
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
	return (id - 1) / 2

def getOriginalId(id):
	return id * 2 + 1