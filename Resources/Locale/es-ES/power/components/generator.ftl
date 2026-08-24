generator-clogged = ¡{CAPITALIZE(THE($generator))} se apaga bruscamente!

portable-generator-verb-start = Encender generador
portable-generator-verb-start-msg-unreliable = Enciende el generador. Puede que necesites varios intentos.
portable-generator-verb-start-msg-reliable = Enciende el generador.
portable-generator-verb-start-msg-unanchored = ¡Primero hay que anclar el generador!
portable-generator-verb-stop = Apagar generador
portable-generator-start-fail = Tiras del cable, pero no arranco.
portable-generator-start-success = Tiras del cable, y cobra vida con un zumbido.

portable-generator-ui-title = Generador portatil
portable-generator-ui-status-stopped = Detenido:
portable-generator-ui-status-starting = Arrancando:
portable-generator-ui-status-running = En marcha:
portable-generator-ui-start = Encender
portable-generator-ui-stop = Apagar
portable-generator-ui-target-power-label = Potencia objetivo (kW):
portable-generator-ui-efficiency-label = Eficiencia:
portable-generator-ui-fuel-use-label = Consumo de combustible:
portable-generator-ui-fuel-left-label = Combustible restante:
portable-generator-ui-clogged = ¡Contaminantes detectados en el tanque de combustible!
portable-generator-ui-eject = Expulsar
portable-generator-ui-eta = (~{ $minutes } min)
portable-generator-ui-unanchored = Sin anclar
portable-generator-ui-current-output = Salida actual: {$voltage}
portable-generator-ui-network-stats = Red:
portable-generator-ui-network-stats-value = { POWERWATTS($supply) } / { POWERWATTS($load) }
portable-generator-ui-network-stats-not-connected = No conectado

power-switchable-generator-examine = La salida de energia esta ajustada a {$voltage}.
power-switchable-generator-switched = ¡Salida cambiada a {$voltage}!

power-switchable-voltage = { $voltage ->
    [HV] [color=orange]AT[/color]
    [MV] [color=yellow]MT[/color]
    *[LV] [color=green]BT[/color]
}
power-switchable-switch-voltage = Cambiar a {$voltage}

fuel-generator-verb-disable-on = ¡Primero apaga el generador!
