# General
rule-restarting-in-seconds =
    Reinicio en { $seconds } { $seconds ->
        [one] segundo
        [few] segundos
       *[other] segundos
    }.
rule-time-has-run-out = ¡Se acabó el tiempo!
# Respawning
rule-respawn-in-seconds =
    Reaparición en { $second } { $second ->
        [one] segundo
        [few] segundos
       *[other] segundos
    }...
