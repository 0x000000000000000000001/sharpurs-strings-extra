open System
open System.Text
open System.Globalization

let isWordChar (str: string) (i: int) : bool =
    let cat = CharUnicodeInfo.GetUnicodeCategory(str, i)
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

let words (s: string) =
    let res = ResizeArray<string>()
    let current = StringBuilder()
    let flush () =
        if current.Length > 0 then
            res.Add(current.ToString())
            current.Clear() |> ignore
    let mutable i = 0
    while i < s.Length do
        if not (isWordChar s i) then
            flush()
            if Char.IsSurrogatePair(s, i) then i <- i + 1
        else
            if i > 0 && current.Length > 0 then
                // Check if previous was lowercase and current is uppercase
                let prevIdx = i - 1
                // Wait, prevIdx might be middle of surrogate
                let isPrevLower = Char.IsLower(s, if prevIdx > 0 && Char.IsSurrogate(s.[prevIdx]) then prevIdx - 1 else prevIdx)
                let isCurrUpper = Char.IsUpper(s, i)
                if isPrevLower && isCurrUpper then
                    flush()
            if Char.IsSurrogatePair(s, i) then
                current.Append(s.[i]) |> ignore
                current.Append(s.[i+1]) |> ignore
                i <- i + 1
            else
                current.Append(s.[i]) |> ignore
        i <- i + 1
    flush()
    res.ToArray()

let camelCase (s: string) =
    let w = words s
    if w.Length = 0 then ""
    else
        let sb = StringBuilder()
        sb.Append(w.[0].ToLowerInvariant()) |> ignore
        for i = 1 to w.Length - 1 do
            let word = w.[i].ToLowerInvariant()
            if word.Length > 0 then
                sb.Append(Char.ToUpperInvariant(word, 0)) |> ignore
                if Char.IsSurrogatePair(word, 0) then
                    sb.Append(word.Substring(2)) |> ignore
                else
                    sb.Append(word.Substring(1)) |> ignore
        sb.ToString()

printfn "1. %s" (camelCase "Thor, Mímir, Ēostre, & Jörð")
printfn "2. %s" (camelCase "🙃, Mímir, ēostre, & Jörð")
printfn "3. %s" (camelCase "thorMímir--Ēostre_Jörð")
