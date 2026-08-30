device-pda-slot-component-slot-name-cartridge = Cartucho
default-program-name = Programa
notekeeper-program-name = Notas
nano-task-program-name = Objetivos
news-read-program-name = Noticias del complejo
crew-manifest-program-name = Manifiesto de la tripulación
crew-manifest-cartridge-loading = Cargando...
net-probe-program-name = Sonda de red
net-probe-scan = ¡Escaneado { $device }!
net-probe-label-name = Nombre
net-probe-label-address = Dirección
net-probe-label-frequency = Frecuencia
net-probe-label-network = Red
log-probe-program-name = Sonda de Log
log-probe-scan = ¡Registros de dispositivos subidos { $device }!
log-probe-label-time = Tiempo
log-probe-label-accessor = Utilizados:
log-probe-label-number = #
log-probe-print-button = Registros de impresión
log-probe-printout-device = Dispositivo escaneado: { $name }
log-probe-printout-header = Últimos registros:
log-probe-printout-entry = #{ $number } / { $time } / { $accessor }
astro-nav-program-name = AstroNav

med-tek-program-name = MedTek

# NanoTask cartridge

nano-task-ui-heading-high-priority-tasks =
    { $amount ->
        [zero] No High Priority Tasks
        [one] 1 High Priority Task
       *[other] {$amount} High Priority Tasks
    }
nano-task-ui-heading-medium-priority-tasks =
    { $amount ->
        [zero] No Medium Priority Tasks
        [one] 1 Medium Priority Task
       *[other] {$amount} Medium Priority Tasks
    }
nano-task-ui-heading-low-priority-tasks =
    { $amount ->
        [zero] No Low Priority Tasks
        [one] 1 Low Priority Task
       *[other] {$amount} Low Priority Tasks
    }
nano-task-ui-done = Completado
nano-task-ui-revert-done = Cancelar
nano-task-ui-priority-low = Bajo
nano-task-ui-priority-medium = Medio
nano-task-ui-priority-high = Alto
nano-task-ui-cancel = Cancelación
nano-task-ui-print = Impresión
nano-task-ui-delete = Borrar
nano-task-ui-save = Salvar
nano-task-ui-new-task = Un nuevo reto
nano-task-ui-description-label = Descripción:
nano-task-ui-description-placeholder = Lleva algo importante
nano-task-ui-requester-label = Solicitante:
nano-task-ui-requester-placeholder = Iván Ivanov
nano-task-ui-item-title = Editar tarea
nano-task-printed-description = Descripción: { $description }
nano-task-printed-requester = Solicitante: { $requester }
nano-task-printed-high-priority = Prioridad: Alta
nano-task-printed-medium-priority = Prioridad: Medio
nano-task-printed-low-priority = Prioridad: Baja
wanted-list-program-name = Buscados
wanted-list-label-no-records = Tranquilo, vaquero
wanted-list-search-placeholder = Buscar por nombre y estado
wanted-list-age-label = [color=darkgray]Age:[/color] [color=white]{ $age }[/color]
wanted-list-job-label = [color=darkgray]Work:[/color] [color=white]{ $job }[/color]
wanted-list-species-label = [color=darkgray]View[/color] [color=white]{ $species }[/color]
wanted-list-gender-label = [color=darkgray]Gender:[/color] [color=white]{ $gender }[/color]
wanted-list-reason-label = [color=darkgray]Cause[/color] [color=white]{ $reason }[/color]
wanted-list-unknown-reason-label = Causa desconocida
wanted-list-initiator-label = [color=darkgray]Initiator:[/color] [color=white]{ $initiator }[/color]
wanted-list-unknown-initiator-label = Iniciador desconocido
wanted-list-status-label = [color=darkgray]Status[/color] { $status ->
        [suspected] [color=yellow]suspect[/color]
        [wanted] [color=red]wanted[/color]
        [detained] [color=#b18644]detained[/color]
        [paroled] [color=green]on parole[/color]
        [discharged] [color=green]released[/color]
       *[other] Sin datos
    }
wanted-list-history-table-time-col = Tiempo
wanted-list-history-table-reason-col = Infracción
wanted-list-history-table-initiator-col = Iniciador
