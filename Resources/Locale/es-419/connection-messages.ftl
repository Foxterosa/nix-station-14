cmd-whitelistadd-desc = Agrega al jugador con el nombre indicado a la whitelist del servidor.
cmd-whitelistadd-help = Uso: whitelistadd <nombre de usuario o ID de usuario>
cmd-whitelistadd-existing = ¡{$username} ya está en la whitelist!
cmd-whitelistadd-added = {$username} fue agregado a la whitelist
cmd-whitelistadd-not-found = No se pudo encontrar a '{$username}'
cmd-whitelistadd-arg-player = [jugador]

cmd-whitelistremove-desc = Quita de la whitelist del servidor al jugador con el nombre indicado.
cmd-whitelistremove-help = Uso: whitelistremove <nombre de usuario o ID de usuario>
cmd-whitelistremove-existing = ¡{$username} no está en la whitelist!
cmd-whitelistremove-removed = {$username} fue quitado de la whitelist
cmd-whitelistremove-not-found = No se pudo encontrar a '{$username}'
cmd-whitelistremove-arg-player = [jugador]

cmd-kicknonwhitelisted-desc = Expulsa del servidor a todos los jugadores que no estén en la whitelist.
cmd-kicknonwhitelisted-help = Uso: kicknonwhitelisted

ban-banned-permanent = Este ban solo se quitará mediante apelación.
ban-banned-permanent-appeal = Este ban solo se quitará mediante apelación. Podés apelar en {$link}
ban-expires = Este ban dura {$duration} minutos y vencerá a las {$time} UTC.
ban-banned-1 = Vos, u otro usuario de esta computadora o conexión, tienen prohibido jugar aquí.
ban-banned-2 = El motivo del ban es: "{$reason}"
ban-banned-3 = Se registrarán los intentos de eludir este ban, como crear una cuenta nueva.

soft-player-cap-full = ¡El servidor está lleno!
panic-bunker-account-denied = El servidor está en modo panic bunker. Pedile a un admin en Discord que te deje entrar, o esperá aproximadamente una hora.
panic-bunker-account-denied-reason = El servidor está en modo panic bunker. Pedile a un admin en Discord que te deje entrar, o esperá aproximadamente una hora. Para omitir siempre el panic bunker, {$reason}
panic-bunker-account-reason-account = tu cuenta de SS14 debe tener más de {$minutes} minutos de antigüedad.
panic-bunker-account-reason-overall = tu tiempo total de juego en este servidor debe ser mayor a {$minutes} {$minutes}.

whitelist-playtime = No tenés suficiente tiempo de juego para entrar a este servidor. Necesitás al menos {$minutes} minutos de juego para entrar.
whitelist-player-count = Este servidor no está aceptando jugadores en este momento. Probá de nuevo más tarde.
whitelist-notes = Actualmente tenés demasiadas notas administrativas para entrar a este servidor. Podés revisar tus notas escribiendo /adminremarks en el chat.
whitelist-manual = No estás en la whitelist de este servidor.
whitelist-blacklisted = Estás en la blacklist de este servidor.
whitelist-always-deny = No tenés permitido unirte a este servidor.
whitelist-fail-prefix = Sin whitelist: {$msg}

cmd-blacklistadd-desc = Agrega al jugador con el nombre indicado a la blacklist del servidor.
cmd-blacklistadd-help = Uso: blacklistadd <nombre de usuario>
cmd-blacklistadd-existing = ¡{$username} ya está en la blacklist!
cmd-blacklistadd-added = {$username} fue agregado a la blacklist
cmd-blacklistadd-not-found = No se pudo encontrar a '{$username}'
cmd-blacklistadd-arg-player = [jugador]

cmd-blacklistremove-desc = Quita de la blacklist del servidor al jugador con el nombre indicado.
cmd-blacklistremove-help = Uso: blacklistremove <nombre de usuario>
cmd-blacklistremove-existing = ¡{$username} no está en la blacklist!
cmd-blacklistremove-removed = {$username} fue quitado de la blacklist
cmd-blacklistremove-not-found = No se pudo encontrar a '{$username}'
cmd-blacklistremove-arg-player = [jugador]

baby-jail-account-denied = Este servidor es para novatos, pensado para jugadores nuevos y para quienes quieran ayudarlos. No se aceptan conexiones nuevas de cuentas demasiado antiguas o que no estén en una whitelist. Probá otros servidores y descubrí todo lo que Space Station 14 tiene para ofrecer. ¡Divertite!
baby-jail-account-denied-reason = Este servidor es para novatos, pensado para jugadores nuevos y para quienes quieran ayudarlos. No se aceptan conexiones nuevas de cuentas demasiado antiguas o que no estén en una whitelist. Probá otros servidores y descubrí todo lo que Space Station 14 tiene para ofrecer. ¡Divertite! Motivo: "{$reason}"
baby-jail-account-reason-account = Tu cuenta de Space Station 14 es demasiado antigua. Debe tener menos de {$minutes} minutos
baby-jail-account-reason-overall = Tu tiempo total de juego en el servidor debe ser menor a {$minutes} {$minutes}

generic-misconfigured = El servidor está mal configurado y no está aceptando jugadores. Contactá al dueño del servidor e intentá de nuevo más tarde.

conntrack-resolve-failed-retry = El servidor no pudo verificar tu conexión en este momento y no tiene una dirección previa registrada para tu cuenta. Reconectate dentro de un rato.

ipintel-server-ratelimited = Este servidor usa un sistema de auditoría con verificación externa, pero alcanzó su límite máximo de verificaciones con el servicio externo. Contactá al equipo administrativo del servidor para avisarles y recibir ayuda, o intentá de nuevo más tarde.
ipintel-unknown = Este servidor usa un sistema de auditoría con verificación externa, pero ocurrió un error al verificar tu conexión. Contactá al equipo administrativo del servidor para avisarles y recibir ayuda, o intentá de nuevo más tarde.
ipintel-suspicious = Parece que estás intentando conectarte usando un datacenter, proxy, VPN u otra conexión sospechosa. Por razones administrativas, no permitimos jugar con esas conexiones. Si tenés una VPN o algo similar activado, apagalo e intentá reconectarte, o contactá al equipo administrativo del servidor si creés que se trata de un falso positivo o necesitás usar esos servicios para jugar.

hwid-required = Tu cliente se negó a enviar un identificador de hardware. Contactá al equipo administrativo para recibir ayuda.
