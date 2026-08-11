open System
open System.Text
open System.Globalization

let isWordChar (c: char) =
    Char.IsLetterOrDigit(c) || Char.IsSymbol(c) || 
    let cat = Char.GetUnicodeCategory(c)
    cat = UnicodeCategory.CurrencySymbol ||
    cat = UnicodeCategory.OtherSymbol ||
    cat = UnicodeCategory.ModifierLetter

let words (s: string) =
    let res = ResizeArray<string>()
    let current = StringBuilder()
    let flush () =
        if current.Length > 0 then
            res.Add(current.ToString())
            current.Clear() |> ignore
    let mutable i = 0
    while i < s.Length do
        let c = s.[i]
        if not (isWordChar c) then
            flush()
        else
            if i > 0 && current.Length > 0 then
                let prev = current.[current.Length - 1]
                if Char.IsLower(prev) && Char.IsUpper(c) then
                    flush()
            current.Append(c) |> ignore
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
                sb.Append(Char.ToUpperInvariant(word.[0])) |> ignore
                sb.Append(word.Substring(1)) |> ignore
        sb.ToString()

printfn "1. %s" (camelCase "Thor, Mímir, Ēostre, & Jörð")
printfn "2. %s" (camelCase "🙃, Mímir, ēostre, & Jörð")
printfn "3. %s" (camelCase "thorMímir--Ēostre_Jörð")
