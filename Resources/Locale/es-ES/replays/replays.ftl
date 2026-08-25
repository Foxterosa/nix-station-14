# Loading Screen

replay-loading = Cargando ({$cur}/{$total})
replay-loading-reading = Leyendo archivos
replay-loading-processing = Procesando archivos
replay-loading-spawning = Generando entidades
replay-loading-initializing = Inicializando entidades
replay-loading-starting= Iniciando entidades
replay-loading-failed = Error al cargar la repeticion. Error:
                        {$reason}
replay-loading-retry = Intentar cargar con mayor tolerancia a excepciones - ¡PUEDE CAUSAR BUGS!
replay-loading-cancel = Cancelar

# Main Menu
replay-menu-subtext = Cliente de repeticiones
replay-menu-load = Cargar repeticion seleccionada
replay-menu-select = Seleccionar una repeticion
replay-menu-open = Abrir carpeta de repeticiones
replay-menu-none = No se encontraron repeticiones.

# Main Menu Info Box
replay-info-title = Informacion de la repeticion
replay-info-none-selected = No hay repeticion seleccionada
replay-info-invalid = [color=red]Se selecciono una repeticion invalida[/color]
replay-info-info = {"["}color=gray]Seleccionada:[/color]  {$name} ({$file})
                   {"["}color=gray]Hora:[/color]   {$time}
                   {"["}color=gray]ID de ronda:[/color]   {$roundId}
                   {"["}color=gray]Duracion:[/color]   {$duration}
                   {"["}color=gray]ForkId:[/color]   {$forkId}
                   {"["}color=gray]Version:[/color]   {$version}
                   {"["}color=gray]Motor:[/color]   {$engVersion}
                   {"["}color=gray]Hash de tipo:[/color]   {$hash}
                   {"["}color=gray]Hash de comp:[/color]   {$compHash}

# Replay selection window
replay-menu-select-title = Seleccionar repeticion

# Replay related verbs
replay-verb-spectate = Espectar

# command
cmd-replay-spectate-help = replay_spectate [entidad opcional]
cmd-replay-spectate-desc = Vincula o desvincula al jugador local a una entidad uid dada.
cmd-replay-spectate-hint = EntityUid opcional
