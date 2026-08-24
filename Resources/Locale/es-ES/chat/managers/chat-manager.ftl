### UI

chat-manager-max-message-length = Tu mensaje supera el límite de {$maxMessageLength} caracteres
chat-manager-ooc-chat-enabled-message = Se ha habilitado el chat OOC.
chat-manager-ooc-chat-disabled-message = Se ha deshabilitado el chat OOC.
chat-manager-looc-chat-enabled-message = Se ha habilitado el chat LOOC.
chat-manager-looc-chat-disabled-message = Se ha deshabilitado el chat LOOC.
chat-manager-dead-looc-chat-enabled-message = Los jugadores muertos ahora pueden usar LOOC.
chat-manager-dead-looc-chat-disabled-message = Los jugadores muertos ya no pueden usar LOOC.
chat-manager-crit-looc-chat-enabled-message = Los jugadores en estado crítico ahora pueden usar LOOC.
chat-manager-crit-looc-chat-disabled-message = Los jugadores en estado crítico ya no pueden usar LOOC.
chat-manager-admin-ooc-chat-enabled-message = Se ha habilitado el chat OOC de admin.
chat-manager-admin-ooc-chat-disabled-message = Se ha deshabilitado el chat OOC de admin.

chat-manager-max-message-length-exceeded-message = Tu mensaje superó el límite de {$limit} caracteres
chat-manager-no-headset-on-message = ¡No llevas puestos unos auriculares!
chat-manager-no-radio-key = ¡No se especificó ninguna clave de radio!
chat-manager-no-such-channel = ¡No existe ningún canal con la clave '{$key}'!
chat-manager-whisper-headset-on-message = ¡No puedes susurrar por la radio!

chat-manager-server-wrap-message = [bold]{$message}[/bold]
chat-manager-sender-announcement = Comando Central

# THE() is not used here because the entity and its name can technically be disconnected if a nameOverride is passed...
chat-manager-entity-me-wrap-message = [italic]{ PROPER($entity) ->
    *[false] El {$entityName} {$message}[/italic]
     [true] {CAPITALIZE($entityName)} {$message}[/italic]
    }

chat-manager-entity-looc-wrap-message = LOOC: [bold]{$entityName}:[/bold] {$message}
chat-manager-send-ooc-patron-wrap-message = OOC: [bold][color={$patronColor}]{$playerName}[/color]:[/bold] {$message}

chat-manager-send-dead-chat-wrap-message = {$deadChannelName}: [bold][BubbleHeader]{$playerName}[/BubbleHeader]:[/bold] [BubbleContent]{$message}[/BubbleContent]
chat-manager-send-admin-dead-chat-wrap-message = {$adminChannelName}: [bold]([BubbleHeader]{$userName}[/BubbleHeader]):[/bold] [BubbleContent]{$message}[/BubbleContent]
chat-manager-send-admin-chat-wrap-message = {$adminChannelName}: [bold]{$playerName}:[/bold] {$message}
chat-manager-send-admin-announcement-wrap-message = [bold]{$adminChannelName}: {$message}[/bold]

chat-manager-send-hook-ooc-wrap-message = OOC: [bold](D){$senderName}:[/bold] {$message}
chat-manager-send-hook-admin-wrap-message = ADMIN: [bold](D){$senderName}:[/bold] {$message}

chat-manager-dead-channel-name = MUERTOS
chat-manager-admin-channel-name = ADMIN

chat-manager-rate-limited = ¡Estás enviando mensajes demasiado rápido!
chat-manager-rate-limit-admin-announcement = Advertencia de límite de mensajes: { $player }

## Speech verbs for chat

chat-speech-verb-suffix-exclamation = !
chat-speech-verb-suffix-exclamation-strong = !!
chat-speech-verb-suffix-question = ?
chat-speech-verb-suffix-stutter = -
chat-speech-verb-suffix-mumble = ..

chat-speech-verb-name-none = Ninguno
chat-speech-verb-name-default = Predeterminado
chat-speech-verb-default = dice
chat-speech-verb-name-exclamation = Exclamando
chat-speech-verb-exclamation = exclama
chat-speech-verb-name-exclamation-strong = Gritando
chat-speech-verb-exclamation-strong = grita
chat-speech-verb-name-question = Preguntando
chat-speech-verb-question = pregunta
chat-speech-verb-name-stutter = Tartamudeando
chat-speech-verb-stutter = tartamudea
chat-speech-verb-name-mumble = Murmurando
chat-speech-verb-mumble = murmura

chat-speech-verb-name-arachnid = Arácnido
chat-speech-verb-insect-1 = chirría
chat-speech-verb-insect-2 = pía
chat-speech-verb-insect-3 = chasquea

chat-speech-verb-name-moth = Polilla
chat-speech-verb-winged-1 = aletea
chat-speech-verb-winged-2 = bate las alas
chat-speech-verb-winged-3 = zumba

chat-speech-verb-name-slime = Slime
chat-speech-verb-slime-1 = chapotea
chat-speech-verb-slime-2 = borbotea
chat-speech-verb-slime-3 = rezuma

chat-speech-verb-name-plant = Diona
chat-speech-verb-plant-1 = susurra
chat-speech-verb-plant-2 = se mece
chat-speech-verb-plant-3 = cruje

chat-speech-verb-name-robotic = Robótico
chat-speech-verb-robotic-1 = declara
chat-speech-verb-robotic-2 = hace bip
chat-speech-verb-robotic-3 = hace bup

chat-speech-verb-name-reptilian = Reptiliano
chat-speech-verb-reptilian-1 = sisea
chat-speech-verb-reptilian-2 = resopla
chat-speech-verb-reptilian-3 = bufa

chat-speech-verb-name-skeleton = Esqueleto
chat-speech-verb-skeleton-1 = traquetea
chat-speech-verb-skeleton-2 = castañetea
chat-speech-verb-skeleton-3 = rechina los dientes

chat-speech-verb-name-vox = Vox
chat-speech-verb-vox-1 = chilla
chat-speech-verb-vox-2 = berrea
chat-speech-verb-vox-3 = grazna

chat-speech-verb-name-canine = Canino
chat-speech-verb-canine-1 = ladra
chat-speech-verb-canine-2 = hace guau
chat-speech-verb-canine-3 = aúlla
# starlight
chat-speech-verb-canine-4 = gañe

chat-speech-verb-name-goat = Cabra
chat-speech-verb-goat-1 = bala
chat-speech-verb-goat-2 = gruñe
chat-speech-verb-goat-3 = berrea

chat-speech-verb-name-small-mob = Ratón
chat-speech-verb-small-mob-1 = chilla
chat-speech-verb-small-mob-2 = hace pip

chat-speech-verb-name-large-mob = Carpa
chat-speech-verb-large-mob-1 = ruge
chat-speech-verb-large-mob-2 = gruñe

chat-speech-verb-name-monkey = Mono
chat-speech-verb-monkey-1 = chimpea
chat-speech-verb-monkey-2 = chilla

chat-speech-verb-name-cluwne = Cluwne

chat-speech-verb-name-parrot = Loro
chat-speech-verb-parrot-1 = grazna
chat-speech-verb-parrot-2 = trina
chat-speech-verb-parrot-3 = pía

chat-speech-verb-cluwne-1 = ríe tontamente
chat-speech-verb-cluwne-2 = suelta una carcajada
chat-speech-verb-cluwne-3 = se ríe

chat-speech-verb-name-ghost = Fantasma
chat-speech-verb-ghost-1 = se queja
chat-speech-verb-ghost-2 = respira
chat-speech-verb-ghost-3 = tararea
chat-speech-verb-ghost-4 = masculla

chat-speech-verb-name-electricity = Electricidad
chat-speech-verb-electricity-1 = chisporrotea
chat-speech-verb-electricity-2 = zumba
chat-speech-verb-electricity-3 = chirría

chat-speech-verb-vulpkanin-1 = hace rawr
chat-speech-verb-vulpkanin-2 = ladra
chat-speech-verb-vulpkanin-3 = hace rur
chat-speech-verb-vulpkanin-4 = gañe
chat-speech-verb-vulpkanin = Vulpkanin

chat-speech-verb-name-wawa = Wawa
chat-speech-verb-wawa-1 = entona
chat-speech-verb-wawa-2 = declara
chat-speech-verb-wawa-3 = proclama
chat-speech-verb-wawa-4 = reflexiona
