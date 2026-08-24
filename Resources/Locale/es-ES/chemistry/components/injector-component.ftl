## UI

injector-volume-transfer-label = Volumen: [color=white]{$currentVolume}/{$totalVolume}u[/color]
    Modo: [color=white]{$modeString}[/color] ([color=white]{$transferVolume}u[/color])
injector-volume-label = Volumen: [color=white]{$currentVolume}/{$totalVolume}u[/color]
    Modo: [color=white]{$modeString}[/color]
injector-toggle-verb-text = Alternar modo del inyector

## Entity

injector-component-inject-mode-name = inyectar
injector-component-draw-mode-name = extraer
injector-component-dynamic-mode-name = dinámico
injector-component-mode-changed-text = Ahora {$mode}
injector-component-transfer-success-message = Transfieres {$amount}u a {THE($target)}.
injector-component-transfer-success-message-self = Te transfieres {$amount}u a ti mismo.
injector-component-inject-success-message = ¡Inyectas {$amount}u en {THE($target)}!
injector-component-inject-success-message-self = ¡Te inyectas {$amount}u!
injector-component-draw-success-message = Extraes {$amount}u de {THE($target)}.
injector-component-draw-success-message-self = Te extraes {$amount}u.

## Fail Messages

injector-component-target-already-full-message = ¡{CAPITALIZE(THE($target))} ya está lleno!
injector-component-target-already-full-message-self = ¡Ya estás lleno!
injector-component-target-is-empty-message = ¡{CAPITALIZE(THE($target))} está vacío!
injector-component-target-is-empty-message-self = ¡Estás vacío!
injector-component-cannot-toggle-draw-message = ¡Demasiado lleno para extraer!
injector-component-cannot-toggle-inject-message = ¡No hay nada que inyectar!
injector-component-cannot-toggle-dynamic-message = ¡No se puede alternar al modo dinámico!
injector-component-empty-message = ¡{CAPITALIZE(THE($injector))} está vacío!
injector-component-blocked-user = ¡El equipo de protección bloqueó tu inyección!
injector-component-blocked-other = ¡La armadura de {CAPITALIZE(THE(POSS-ADJ($target)))} bloqueó la inyección de {THE($user)}!
injector-component-cannot-transfer-message = ¡No puedes transferir a {THE($target)}!
injector-component-cannot-transfer-message-self = ¡No puedes transferirte a ti mismo!
injector-component-cannot-inject-message = ¡No puedes inyectar en {THE($target)}!
injector-component-cannot-inject-message-self = ¡No puedes inyectarte a ti mismo!
injector-component-cannot-draw-message = ¡No puedes extraer de {THE($target)}!
injector-component-cannot-draw-message-self = ¡No puedes extraerte a ti mismo!
injector-component-ignore-mobs = ¡Este inyector solo puede interactuar con contenedores!

## mob-inject doafter messages

injector-component-needle-injecting-user = Empiezas a inyectar la aguja.
injector-component-needle-injecting-target = ¡{CAPITALIZE(THE($user))} está intentando inyectarte una aguja!
injector-component-needle-drawing-user = Empiezas a extraer con la aguja.
injector-component-needle-drawing-target = ¡{CAPITALIZE(THE($user))} está intentando extraerte sangre con una aguja!
injector-component-spray-injecting-user = Empiezas a preparar la boquilla del aerosol.
injector-component-spray-injecting-target = ¡{CAPITALIZE(THE($user))} está intentando ponerte una boquilla de aerosol!

## Target Popup Success messages
injector-component-feel-prick-message = ¡Sientes un pinchazo diminuto!
