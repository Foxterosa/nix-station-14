game-ticker-restart-round = Reiniciando la ronda...
game-ticker-start-round = Iniciando la ronda...
game-ticker-start-round-cannot-start-game-mode-fallback = ¡No se pudo iniciar el modo { $failedGameMode }! Iniciando { $fallbackMode }...
game-ticker-start-round-cannot-start-game-mode-restart = ¡No se pudo iniciar el modo { $failedGameMode }! Reiniciando la ronda...
game-ticker-start-round-invalid-map = El mapa seleccionado, { $map }, no es adecuado para el modo de juego { $mode }. Es posible que el modo de juego no funcione correctamente.
game-ticker-unknown-role = Desconocido
game-ticker-delay-start = El inicio de la ronda se retrasó { $seconds } segundos.
game-ticker-pause-start = El inicio de la ronda está en pausa.
game-ticker-pause-start-resumed = Se reanudó la cuenta regresiva para el inicio de la ronda.
game-ticker-player-join-game-message = ¡Bienvenido a Nix Station! Basado en Starlight! Si es tu primera vez, presiona ESC y lee las reglas del juego. Si necesitas ayuda, no dudes en utilizar AHelp.
game-ticker-get-info-text =
    Ronda actual: [color=white]#{ $roundId }[/color]
    Jugadores actuales: [color=white]{ $playerCount }[/color]
    Mapa actual: [color=white]{ $mapName }[/color]
    Modo de juego actual: [color=white]{ $gmTitle }[/color]
    >[color=yellow]{ $desc }[/color]
game-ticker-get-info-preround-text =
    Ronda actual: [color=white]#{ $roundId }[/color]
    Jugadores actuales: [color=white]{ $playerCount }[/color] ([color=white]{ $readyCount }[/color] { $readyCount ->
        [one] listo
       *[other] listos
    })
    Mapa actual: [color=white]{ $mapName }[/color]
    Modo de juego actual: [color=white]{ $gmTitle }[/color]
    >[color=yellow]{ $desc }[/color]
game-ticker-no-map-selected = ¡[color=red]El mapa aún no ha sido seleccionado.[/color]
game-ticker-player-no-jobs-available-when-joining = No había puestos disponibles al intentar unirse a la partida.
# Displayed in chat to admins when a player joins
game-ticker-player-no-character-for-job-available-when-joining = When attempting to join the game, no characters were available for selected job {$job}.

# Displayed in chat to admins when a player joins
player-join-message = El jugador { $name } se conectó.
player-first-join-message = El jugador { $name } se conectó al servidor por primera vez.
# Displayed in chat to admins when a player leaves
player-leave-message = El jugador { $name } se desconectó.
latejoin-arrival-announcement = {$character} ({$job}) ha llegado a la estación.
latejoin-arrival-announcement-special = {$job} {$character} se ha presentado.
latejoin-arrival-sender = Estación
latejoin-arrivals-direction = Una nave de transporte hacia tu estación llegará en breve.
latejoin-arrivals-direction-time = Una nave de transporte hacia tu estación llegará en {$time}.
latejoin-arrivals-dumped-from-shuttle = Una fuerza misteriosa te impide salir con la nave de llegadas.
latejoin-arrivals-teleport-to-spawn = Una fuerza misteriosa te teletransporta fuera de la nave de llegadas. Que tengas un buen turno.

preset-not-enough-ready-players = No se pudo iniciar el preajuste { $presetName }. Requiere { $minimumPlayers } jugadores, pero solo { $readyPlayersCount } están listos.
preset-no-one-ready = No se pudo iniciar el modo { $presetName }. No hay jugadores listos.
game-run-level-PreRoundLobby = Lobby previo a la ronda
game-run-level-InRound = Ronda en curso
game-run-level-PostRound = Después de la ronda
