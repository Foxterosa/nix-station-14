# Examine text after when they're holding something (in-hand)
comp-hands-examine = { CAPITALIZE(SUBJECT($user)) } está sosteniendo { $items }.
comp-hands-examine-empty = { CAPITALIZE(SUBJECT($user)) } no está sosteniendo nada.
comp-hands-examine-wrapper = { INDEFINITE($item) } [color=paleturquoise]{$item}[/color]

hands-system-blocked-by = Bloqueado por
