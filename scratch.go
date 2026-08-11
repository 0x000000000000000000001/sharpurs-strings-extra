package main
import (
	"fmt"
	"unicode"
)

func isWordChar(r rune) bool {
	return unicode.IsLetter(r) || unicode.IsNumber(r) || unicode.IsSymbol(r) || unicode.Is(unicode.Sc, r) || unicode.Is(unicode.So, r) || unicode.Is(unicode.Lm, r)
}

func main() {
    fmt.Println("🙃 isWordChar:", isWordChar('🙃'))
}
