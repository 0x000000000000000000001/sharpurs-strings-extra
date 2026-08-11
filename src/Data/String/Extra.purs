module Data.String.Extra
  ( camelCase
  , kebabCase
  , pascalCase
  , snakeCase
  , upperCaseFirst
  , words
  , levenshtein
  , sorensenDiceCoefficient
  ) where

-- | Converts a `String` to camel case
-- |
-- | ```purs
-- | camelCase "Hello world" == "helloWorld"
-- | ```
foreign import camelCase :: String -> String

-- | Converts a `String` to kebab case
-- |
-- | ```purs
-- | kebabCase "Hello world" == "hello-world"
-- | ```
foreign import kebabCase :: String -> String

-- | Converts a `String` to Pascal case
-- |
-- | ```purs
-- | pascalCase "Hello world" == "HelloWorld"
-- | ```
foreign import pascalCase :: String -> String

-- | Converts a `String` to snake case
-- |
-- | ```purs
-- | snakeCase "Hello world" == "hello_world"
-- | ```
foreign import snakeCase :: String -> String

-- | Converts the first character in a `String` to upper case, lower-casing
-- | the rest of the string.
-- |
-- | ```purs
-- | upperCaseFirst "hello World" == "Hello world"
-- | ```
foreign import upperCaseFirst :: String -> String

-- | Separates a `String` into words based on Unicode separators, capital
-- | letters, dashes, underscores, etc.
-- |
-- | ```purs
-- | words "Hello_world --from TheAliens" == [ "Hello", "world", "from", "The", "Aliens" ]
-- | ```
foreign import words :: String -> Array String

-- | Calculates the Levenshtein distance between two strings.
-- |
-- | ```purs
-- | levenshtein "book" "back" -- 2
-- | ```
foreign import levenshtein :: String -> String -> Int

-- | Calculates the Sørensen-Dice coefficient between two strings.
-- |
-- | ```purs
-- | sorensenDiceCoefficient "WHIRLED" "WORLD" -- 0.2000
-- | ```
foreign import sorensenDiceCoefficient :: String -> String -> Number
