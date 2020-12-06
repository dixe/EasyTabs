namespace EasyTabs.Parser

module ParserFs =
    open FParsec
    open System.Text

    type GuitarString = LowE | A | D | G | B | HighE | Pause

    type Fret = Fret of int | Pause
    type Note = { Fret : Fret;  String : GuitarString  }

    type Moment = { Notes : Note List}

    type Part = {Name : string option; Moments : Moment List}

    type ParserResultFs = {Parts : Part List}

    type version = V1 | V2 | V3
    
    
    
    let parseLabel = 
        let labelChar c = not (c=']')
        opt (pstring "[" >>. many1Satisfy labelChar)

    let convertToNote n =        
        match n with
        | 'e' -> HighE
        | 'B' -> B
        | 'G' -> G
        | 'D' -> D
        | 'A' -> A
        | 'E' -> LowE
        | x ->  HighE // TODO return some kind of error

    let convertToNotes s = Seq.map convertToNote (Seq.toList s) 


    let validString n = n='e' || n='B' || n='G' || n='D' || n='A' || n='E'

    let parseStrings = many1Satisfy validString

    let convertFretNum n = Fret n

    let convertFretPause _ = Pause

    let parseFret = (pint32 |>> convertFretNum) <|> (pstring "-" |>> convertFretPause)

    let parseFrets n = many1 (parray n  (parseFret .>> spaces))

    let createNote (string, fret) = { Fret = fret; String = string}

    let makeSeq strings frets = 
        let zipped = Seq.zip strings frets
        List.ofSeq (Seq.map createNote zipped)

    let rec createNotes strings frets =    
        match frets with 
        | [] -> []
        | f::fs -> (makeSeq strings f) @ (createNotes strings fs)
       

    let parseMoment strings = 
        let count = Seq.length strings
        (parseFrets count)  |>> (createNotes strings)

    let createMoment notes = {Notes = notes}

    let parseMoments = many1 (((parseStrings |>> convertToNotes) .>> spaces) >>= parseMoment |>> createMoment)


    let createPart label moments = {Name= label; Moments = moments}

    let createParserResult parts = {Parts = parts}

    let v1Parser = many1 (spaces >>. pipe2 parseLabel parseMoments createPart) |>> createParserResult

    let versionParser = pchar 'v' >>. pint32 

    let versionChoice v = 
        match v with
        | 1 -> v1Parser
        | x -> fail (sprintf "version=%i is not supported" x)

    let easyTabParser = versionParser >>= versionChoice .>> eof


    let ParseEasyTabs str = 
        match run easyTabParser str with
        | Success(result,s, d)   -> Right result
        | Failure(errorMsg, _, _) -> Left errorMsg


    let TestParse str  =
        match run easyTabParser str with
        | Success(result,s, d)   -> printfn "Success: %A %A %A"  result s d
        | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

