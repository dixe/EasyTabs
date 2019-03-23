import sys


def parse(tokens, resDict):

    print tokens


    state = []

    while tokens != []:
        reqTokens = []
        token, tokens = next_token(tokens)
        if token[0].isalpha():
            state = []
            print token
            exit()
            for char in token:
                if char.isalpha():
                    state.append(char)
        else:
            reqTokens = [token]


        tokens, resDict = parse_state(state, tokens, reqTokens, resDict)


    return resDict

def parse_state(state, tokens, reqTokens, resDict):


    while len(reqTokens)< len(state):

        token, tokens = next_token(tokens)
        reqTokens.append(token)


    stateNote = dict(zip(state, reqTokens))
    for k in resDict:
        if k in stateNote:
            resDict[k] += stateNote[k].rjust(2,'-') + "-"

        else:
            resDict[k] += "---"
    return tokens, resDict


def next_token(tokens):
    if tokens == []:
        return None, []
    return tokens[0], tokens[1:]


def resDictToString(resDict, maxLineLen):
    notes = "e B G D A E".split()
    res = ""

    for i in range(0, max(len(resDict[resDict.keys()[0]]),1), maxLinelen):
        for n in notes:
            res += "{0}|{1}\n".format(n, resDict[n][i:i+maxLinelen])
        res += "\n\n"
    return res.strip()


test = "G 12 A 12 11 12 9 7 9"

testChord = "eBGD 2 3 2 0"

powerChordTest = "EA 9 11 9 11 G 9 9 9 11 7 11 12 11 9 7"

# use maybe S for or R after a chord name to indetify root position chord to avoid having to write it all out so fx CO and other build in chords, but not important since we are not using it for chords that much


def parseInputFile(path):
    with open(sys.argv[1],'r') as f:

        lines = f.readlines()
        version = lines[0].strip()


    if version == 'v0':
        content = map(str.strip, lines[1:])

    if version == 'v1':
        content = parseV1(lines[1:])

    maxLinelen = 60 # todo make this notes or bars, instead of chars, also makes parsing harder, add in new parse file versio

    return content, maxLinelen

def parseV1(lines):
    # allow insert of label [verse] with value in between to mark in output

    tokens = []
    for l in lines:
        if l.startswith('['):
            label = l[1:-2]
            tokens.append(("label", label))
        else:
            tokens.append(("note", l.split()))

    return tokens







if len(sys.argv) == 2:

    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    text, maxLinelen =  parseInputFile(sys.argv)

    resDict = parse(text, baseDict)
    print resDictToString(resDict, maxLinelen)
else:
    print " old test "
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(powerChordTest, baseDict))
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(test, baseDict))
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(testChord, baseDict))
