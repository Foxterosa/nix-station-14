### UI

# Shown when a stack is examined in details range
comp-stack-examine-detail-count = {$count ->
    [one] Hay [color={$markupCountColor}]{$count}[/color] unidad
    *[other] Hay [color={$markupCountColor}]{$count}[/color] unidades
} en la pila.

# Stack status control
comp-stack-status = Cantidad: [color=white]{$count}[/color]

### Interaction Messages

# Shown when attempting to add to a stack that is full
comp-stack-already-full = La pila ya está llena.

# Shown when a stack becomes full
comp-stack-becomes-full = La pila ahora está llena.

# Text related to splitting a stack
comp-stack-split = Divides la pila.
comp-stack-split-halve = Partir a la mitad
comp-stack-split-too-small = La pila es demasiado pequeña para dividirla.
