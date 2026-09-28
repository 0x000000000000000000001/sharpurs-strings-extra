module Data_String_Extra_FFI

open System
open System.Text
open System.Globalization
open System.Collections.Generic

let private isWordChar (s: string) : bool =
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

let private isUpper (s: string) =
    if s.Length = 0 then false else Char.IsUpper(s, 0)

let private isLower (s: string) =
    if s.Length = 0 then false else Char.IsLower(s, 0)

let private isDigit (s: string) =
    if s.Length = 0 then false else Char.IsDigit(s, 0)

let private isLetter (s: string) =
    if s.Length = 0 then false else Char.IsLetter(s, 0)

let words (str: obj) =
    let s = unbox<string> str
    let res = ResizeArray<string>()
    let current = StringBuilder()
    
    let flush () =
        if current.Length > 0 then
            res.Add(current.ToString())
            current.Clear() |> ignore

    let enumerator = StringInfo.GetTextElementEnumerator(s)
    let mutable prev = ""
    while enumerator.MoveNext() do
        let el = enumerator.GetTextElement()
        if not (isWordChar el) then
            flush()
        else
            if prev <> "" && current.Length > 0 then
                let boundary =
                    (isLower prev && isUpper el)
                        || (isDigit prev && isLetter el)
                        || (isLetter prev && isDigit el)
                if boundary then
                    flush()
            current.Append(el) |> ignore
        prev <- el
    
    flush()
    box (res.ToArray())

let camelCase (str: obj) =
    let w = unbox<string[]> (words str)
    if w.Length = 0 then box ""
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
        box (sb.ToString())

let pascalCase (str: obj) =
    let w = unbox<string[]> (words str)
    if w.Length = 0 then box ""
    else
        let sb = StringBuilder()
        for i = 0 to w.Length - 1 do
            let word = w.[i].ToLowerInvariant()
            if word.Length > 0 then
                let enumerator = StringInfo.GetTextElementEnumerator(word)
                if enumerator.MoveNext() then
                    let first = enumerator.GetTextElement()
                    sb.Append(first.ToUpperInvariant()) |> ignore
                    sb.Append(word.Substring(first.Length)) |> ignore
        box (sb.ToString())

let kebabCase (str: obj) =
    let w = unbox<string[]> (words str)
    let lower = w |> Array.map (fun x -> x.ToLowerInvariant())
    box (String.Join("-", lower))

let snakeCase (str: obj) =
    let w = unbox<string[]> (words str)
    let lower = w |> Array.map (fun x -> x.ToLowerInvariant())
    box (String.Join("_", lower))

let upperCaseFirst (str: obj) =
    let s = unbox<string> str
    if s.Length = 0 then box s
    else
        let enumerator = StringInfo.GetTextElementEnumerator(s)
        if enumerator.MoveNext() then
            let first = enumerator.GetTextElement()
            let rest = s.Substring(first.Length).ToLowerInvariant()
            box (first.ToUpperInvariant() + rest)
        else box s

let levenshtein = fun (s1_obj: obj) -> fun (s2_obj: obj) ->
    let s1 = unbox<string> s1_obj
    let s2 = unbox<string> s2_obj
    let len1 = s1.Length
    let len2 = s2.Length
    if len1 = 0 then box len2
    elif len2 = 0 then box len1
    else
        let d = Array2D.create (len1 + 1) (len2 + 1) 0
        for i = 0 to len1 do d.[i, 0] <- i
        for j = 0 to len2 do d.[0, j] <- j
        for i = 1 to len1 do
            for j = 1 to len2 do
                let cost = if s1.[i - 1] = s2.[j - 1] then 0 else 1
                let min1 = d.[i - 1, j] + 1
                let min2 = d.[i, j - 1] + 1
                let min3 = d.[i - 1, j - 1] + cost
                d.[i, j] <- Math.Min(min1, Math.Min(min2, min3))
        box d.[len1, len2]

let sorensenDiceCoefficient = fun (s1_obj: obj) -> fun (s2_obj: obj) ->
    let s1 = unbox<string> s1_obj
    let s2 = unbox<string> s2_obj
    if s1.Length < 2 || s2.Length < 2 then box 0.0
    else
        let bigrams = Dictionary<string, int>()
        for i = 0 to s1.Length - 2 do
            let bg = s1.Substring(i, 2)
            if bigrams.ContainsKey(bg) then
                bigrams.[bg] <- bigrams.[bg] + 1
            else
                bigrams.[bg] <- 1

        let mutable intersection = 0
        for i = 0 to s2.Length - 2 do
            let bg = s2.Substring(i, 2)
            if bigrams.ContainsKey(bg) && bigrams.[bg] > 0 then
                bigrams.[bg] <- bigrams.[bg] - 1
                intersection <- intersection + 1
        
        box ((2.0 * float intersection) / float (s1.Length + s2.Length - 2))
