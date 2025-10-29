## ISO 7185:1990 (Pascal Standard)

## Описанные ликсемы:
    * Идентификатор
    * Литерал целого числа
    * Литерал числа с плавающей точкой
    * Литерал строки

`letter` = 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n' | 'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x' | 'y' | 'z'

`digit` = '0' | '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9'

`sign` = '+' | '-' 

### Идентификатор
`identifier` = `letter` {`letter` ∣ `digit` }

### Литерал целого числа

`unsigned-integer` = `digit-sequence`

`signed-integer` = [ `sign` ] `unsigned-integer`

`digit-sequence` = `digit` { `digit` }

### Литерал числа с плавающей точкой

`unsigned-real` = `digit-sequence`.`fractional-part` [ e`scale-factor` ] 
                | `digit-sequence`e`scale-factor`

`signed-real` = [ `sign` ] `unsigned-real`

`scale-factor` = [ `sign` ] `digit-sequence`

`fractional-part` = `digit-sequence`

### Литерал строки

`character-string` = "'" `string-element` { `string-element` } "'"

`string-element` = `apostrophe-image` | `string-character` 

`apostrophe-image` = "'"
