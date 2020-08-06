![](https://storage.googleapis.com/replit/images/1596342110450_7674509b2f36a76d1c139806e2b0cf08.jpeg)
# Obratnaya [oʊbɹɑtnɑjɑ]

*You must be install .NET or mono*

## The Section
Define data / main section
```
.data:
    .text Text data
    .text unicode$<FFFD>
    .decimal 3.14159265
    .boolean 1
    .empty
.main:
    mov 0,@
    ; Get value from data section and put to stack
    msg @
    ; Print the value at the top of the stack
```

## Words

### mov
Move data section's data to stack
```
mov 0,@
; mov pos,@
```

### movt
Move the text value at the top of the stack to data section
```
movt @,0
; movt @,pos
```

### movd
Move the decimal value at the top of the stack to data section
```
movd @,0
; movd @,pos
```

### movb
Move the boolean value at the top of the stack to data section
```
movb @,0
; movb @,pos
```

### msg
Prints the value at the top of the stack **without newline**
```
msg @
; msg @
```

### add
Adds the decimal value at the top of the stack and the decimal value at the 2nd from top of the stack
```
add @,@
add 0,@
add @,0
add 0,0
; add @,@
; add decimal,@
; add @,decimal
; add decimal,decimal
```

### sub
Subtracts the decimal value at the top of the stack by the decimal value at the 2nd from top of the stack
```
sub @,@
sub 0,@
sub @,0
sub 0,0
; sub @,@
; sub decimal,@
; sub @,decimal
; sub decimal,decimal
```

### mul
Multiplys the decimal value at the top of the stack and the decimal value at the 2nd from top of the stack
```
mul @,@
mul 0,@
mul @,0
mul 0,0
; mul @,@
; mul decimal,@
; mul @,decimal
; mul decimal,decimal
```

### div
Divides the decimal value at the top of the stack by the decimal value at the 2nd from top of the stack
```
div @,@
div 0,@
div @,0
div 0,0
; div @,@
; div decimal,@
; div @,decimal
; div decimal,decimal
```

### mod
Modulos the decimal value at the top of the stack by the decimal value at the 2nd from top of the stack
```
mod @,@
mod 0,@
mod @,0
mod 0,0
; mod @,@
; mod decimal,@
; mod @,decimal
; mod decimal,decimal
```

### and
Calculates AND logic by the boolean value at the top of the stack and the boolean value at the 2nd from top of the stack
```
and @,@
and 0,@
and @,0
and 0,0
; and @,@
; and boolean,@
; and @,boolean
; and boolean,boolean
```

### or
Calculates OR logic by the boolean value at the top of the stack and the boolean value at the 2nd from top of the stack
```
or @,@
or 0,@
or @,0
or 0,0
; or @,@
; or boolean,@
; or @,boolean
; or boolean,boolean
```

### xor
Calculates XOR logic by the boolean value at the top of the stack and the boolean value at the 2nd from top of the stack
```
xor @,@
xor 0,@
xor @,0
xor 0,0
; xor @,@
; xor boolean,@
; xor @,boolean
; xor boolean,boolean
```

### not
Calculates NOT logic by the boolean value at the top of the stack
```
not @
not 0
; not @
; not boolean
```

### dup
Duplicate the value at the top of the stack
```
dup
```

### pop
Pops the value at the top of the stack
```
pop
```

### remove
Removes the data section's data
```
remove @
remove 0
; remove @
; remove decimal(uint)
```

### ret
Exits program
```
ret
ret @
ret 0
; ret
; ret @
; ret decimal
```

### jmp (not supporting in transpile mode)
Jumps to line
```
jmp @
jmp 0
; jmp @
; jmp decimal
```

### concat
Concatenates the value at the top of the stack and the value at the 2nd from top of the stack
```
concat @,@
concat 0,@
concat @,0
concat 0,0
; concat @,@
; concat any types,@
; concat @,any types
; concat any types,any types
```

### emit
Emits text by the value at the top of the stack
```
emit @
emit 0
; emit @
; emit decimal(uint)
```

### ;
Comment
```
; comment
msg @ ; comment
; ^ error
```

### /* ~ */
Multiline comment
```
/*hello
world*/
/*
hello
world
*/
/*hello world*/
msg @ /*hello
world*/
; ^ error
```

### nop
Do nothing
```
nop
nop 0
; nop
; nop any types[, any types[, any types[, ...]]]
```

### push
Push value to the stack by type `undefined`
```
push @
; push @
```

### type
Get type of the value at the top of the stack
```
type
; type
```

### cr
Prints carriage return (U+000D)
```
cr
; cr
```

### lf
Prints line feed (U+000A)
```
lf
; lf
```