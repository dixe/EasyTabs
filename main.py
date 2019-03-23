import sys
from copy import deepcopy


NOTES_NAME = "NOTES"

LABEL_NAME = "LABEL"



def parse(tokenTuples, resDict):


    state = []

    outputRes = []
    while tokenTuples != []:
        reqTokens = []

        tokenTup, tokenTuples = next_token(tokenTuples)

        if tokenTup[0] == NOTES_NAME:
            stateDict = deepcopy(resDict)
            tokens = tokenTup[1]

            while tokens != []:
                token, tokens = next_token(tokens)
                if token[0].isalpha():
                    state = []
                    for char in token:
                        if char.isalpha():
                            state.append(char)
                else:
                    reqTokens = [token]


                tokens, stateDict = parse_state(state, tokens, reqTokens, stateDict)

            outputRes.append((tokenTup[0], stateDict))
        elif tokenTup[0] == LABEL_NAME:
            outputRes.append(tokenTup)

    return outputRes

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



def listResDictToString(resList):
    notes = "e B G D A E".split()
    res = ""

    for tokenTup in resList:
        if tokenTup[0] == NOTES_NAME:

            res += resDictToString(tokenTup[1])
            res += '\n\n'
        elif tokenTup[0] == LABEL_NAME:
            res += tokenTup[1] + '\n'


    return res

def resDictToString(resDict):
    notes = "e B G D A E".split()
    res = ""

    for n in notes:
        res += "{0}|{1}\n".format(n, resDict[n])
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
        if len(l.strip()) == 0:
            continue
        if l.startswith('['):
            label = l[1:-2]
            tokens.append((LABEL_NAME, label))
        else:
            tokens.append((NOTES_NAME, l.split()))

    return tokens







if len(sys.argv) == 2:

    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    text, maxLinelen =  parseInputFile(sys.argv)

    resList = parse(text, baseDict)
    print listResDictToString(resList)
else:
    print " old test "
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(powerChordTest, baseDict))
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(test, baseDict))
    baseDict = {'E':"", 'A':"", 'D' :"",'G':"",'B':"",'e':""}
    print resDictToString(parse(testChord, baseDict))
