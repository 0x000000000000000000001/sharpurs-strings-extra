open System
open System.Text
open System.Globalization

let isWordChar (s: string) : bool =
    if s.Length = 0 then false
    else
        let cat = CharUnicodeInfo.GetUnicodeCategory(s, 0)
        cat = UnicodeCategory.UppercaseLetter ||
        cat = UnicodeCategory.LowercaseLetter ||
        cat = UnicodeCategory.TitlecaseLetter ||
        cat = UnicodeCategory.ModifierLetter ||
        cat = UnicodeCategory.OtherLetter ||
        cat = UnicodeCategory.DecimalDigitNumber ||
        cat = UnicodeCategory.LetterNumber ||
        cat = UnicodeCategory.OtherNumber ||
        cat = UnicodeCategory.MathSymbol ||
        cat = UnicodeCategory.CurrencySymbol ||
        cat = UnicodeCategory.ModifierSymbol ||
        cat = UnicodeCategory.OtherSymbol

let isUpper (s: string) =
    if s.Length = 0 then false else Char.IsUpper(s, 0)

let isLower (s: string) =
    if s.Length = 0 then false else Char.IsLower(s, 0)

let words (str: string) =
    let res = ResizeArray<string>()
    let current = StringBuilder()
    let flush () =
        if current.Length > 0 then
            res.Add(current.ToString())
            current.Clear() |> ignore
            
    let enumerator = StringInfo.GetTextElementEnumerator(str)
    let mutable prev = ""
    while enumerator.MoveNext() do
        let s = enumerator.GetTextElement()
        if not (isWordChar s) then
            flush()
        else
            if prev <> "" && current.Length > 0 then
                if isLower prev && isUpper s then
                    flush()
            current.Append(s) |> ignore
        prev <- s
    flush()
    res.ToArray()

let camelCase (str: string) =
    let w = words str
    if w.Length = 0 then ""
    else
        let sb = StringBuilder()
        sb.Append(w.[0].ToLowerInvariant()) |> ignore
        for i = 1 to w.Length - 1 do
            let word = w.[i].ToLowerInvariant()
            if word.Length > 0 then
                let enumerator = StringInfo.GetTextElementEnumerator(word)
                if enumerator.MoveNext() then
                    let first = enumerator.GetTextElement()
                    sb.Append(first.ToUpperInvariant()) |> ignore
                    sb.Append(word.Substring(first.Length)) |> ignore
        sb.ToString()

printfn "1. %s" (camelCase "Thor, Mímir, Ēostre, & Jörð")
printfn "2. %s" (camelCase "🙃, Mímir, ēostre, & Jörð")
printfn "3. %s" (camelCase "thorMímir--Ēostre_Jörð")
