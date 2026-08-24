-create-3rd-person =
    { $chance ->
        [1] Crea
        *[other] crear
    }

-cause-3rd-person =
    { $chance ->
        [1] Provoca
        *[other] provocar
    }

-satiate-3rd-person =
    { $chance ->
        [1] Sacia
        *[other] saciar
    }

entity-effect-guidebook-spawn-entity =
    { $chance ->
        [1] Crea
        *[other] crear
    } { $amount ->
        [1] {INDEFINITE($entname)}
        *[other] {$amount} {$entname}
    }

entity-effect-guidebook-destroy =
    { $chance ->
        [1] Destruye
        *[other] destruir
    } el objeto

entity-effect-guidebook-break =
    { $chance ->
        [1] Rompe
        *[other] romper
    } el objeto

entity-effect-guidebook-explosion =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } una explosión

entity-effect-guidebook-emp =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } un pulso electromagnético

entity-effect-guidebook-flash =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } un destello cegador

entity-effect-guidebook-foam-area =
    { $chance ->
        [1] Crea
        *[other] crear
    } grandes cantidades de espuma

entity-effect-guidebook-smoke-area =
    { $chance ->
        [1] Crea
        *[other] crear
    } grandes cantidades de humo

entity-effect-guidebook-satiate-thirst =
    { $chance ->
        [1] Sacia
        *[other] saciar
    } { $relative ->
        [1] la sed a un ritmo medio
        *[other] la sed a {NATURALFIXED($relative, 3)}x el ritmo medio
    }

entity-effect-guidebook-satiate-hunger =
    { $chance ->
        [1] Sacia
        *[other] saciar
    } { $relative ->
        [1] el hambre a un ritmo medio
        *[other] el hambre a {NATURALFIXED($relative, 3)}x el ritmo medio
    }

entity-effect-guidebook-health-change =
    { $chance ->
        [1] { $healsordeals ->
                [heals] Cura
                [deals] Inflige
                *[both] Modifica la salud en
             }
        *[other] { $healsordeals ->
                    [heals] curar
                    [deals] infligir
                    *[both] modificar la salud en
                 }
    } { $changes }

entity-effect-guidebook-even-health-change =
    { $chance ->
        [1] { $healsordeals ->
            [heals] Cura de forma pareja
            [deals] Inflige de forma pareja
            *[both] Modifica de forma pareja la salud en
        }
        *[other] { $healsordeals ->
            [heals] curar de forma pareja
            [deals] infligir de forma pareja
            *[both] modificar de forma pareja la salud en
        }
    } { $changes }

entity-effect-guidebook-status-effect-old =
    { $type ->
        [update]{ $chance ->
                    [1] Provoca
                     *[other] provocar
                 } {LOC($key)} durante al menos {NATURALFIXED($time, 3)} segundos sin acumulación
        [add]   { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {LOC($key)} durante al menos {NATURALFIXED($time, 3)} segundos con acumulación
        [set]  { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {LOC($key)} durante {NATURALFIXED($time, 3)} segundos sin acumulación
        *[remove]{ $chance ->
                    [1] Quita
                    *[other] quitar
                } {NATURALFIXED($time, 3)} segundos de {LOC($key)}
    }

entity-effect-guidebook-status-effect =
    { $type ->
        [update]{ $chance ->
                    [1] Provoca
                    *[other] provocar
                 } {$key} durante al menos {NATURALFIXED($time, 3)} segundos sin acumulación
        [add]   { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {$key} durante al menos {NATURALFIXED($time, 3)} segundos con acumulación
        [set]  { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {$key} durante al menos {NATURALFIXED($time, 3)} segundos sin acumulación
        *[remove]{ $chance ->
                    [1] Quita
                    *[other] quitar
                } {NATURALFIXED($time, 3)} segundos de {$key}
    } { $delay ->
        [0] de inmediato
        *[other] tras un retraso de {NATURALFIXED($delay, 3)} segundos
    }

entity-effect-guidebook-status-effect-indef =
    { $type ->
        [update]{ $chance ->
                    [1] Provoca
                    *[other] provocar
                 } {$key} de forma permanente
        [add]   { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {$key} de forma permanente
        [set]  { $chance ->
                    [1] Provoca
                    *[other] provocar
                } {$key} de forma permanente
        *[remove]{ $chance ->
                    [1] Quita
                    *[other] quitar
                } {$key}
    } { $delay ->
        [0] de inmediato
        *[other] tras un retraso de {NATURALFIXED($delay, 3)} segundos
    }

entity-effect-guidebook-knockdown =
    { $type ->
        [update]{ $chance ->
                    [1] Provoca
                    *[other] provocar
                    } {LOC($key)} durante al menos {NATURALFIXED($time, 3)} segundos sin acumulación
        [add]   { $chance ->
                    [1] Provoca
                    *[other] provocar
                } derribo durante al menos {NATURALFIXED($time, 3)} segundos con acumulación
        *[set]  { $chance ->
                    [1] Provoca
                    *[other] provocar
                } derribo durante al menos {NATURALFIXED($time, 3)} segundos sin acumulación
        [remove]{ $chance ->
                    [1] Quita
                    *[other] quitar
                } {NATURALFIXED($time, 3)} segundos de derribo
    }

entity-effect-guidebook-set-solution-temperature-effect =
    { $chance ->
        [1] Fija
        *[other] fijar
    } la temperatura de la solución exactamente en {NATURALFIXED($temperature, 2)}k

entity-effect-guidebook-adjust-solution-temperature-effect =
    { $chance ->
        [1] { $deltasign ->
                [1] Agrega
                *[-1] Quita
            }
        *[other]
            { $deltasign ->
                [1] agregar
                *[-1] quitar
            }
    } calor a la solución hasta que alcance { $deltasign ->
                [1] como máximo {NATURALFIXED($maxtemp, 2)}k
                *[-1] al menos {NATURALFIXED($mintemp, 2)}k
            }

entity-effect-guidebook-adjust-reagent-reagent =
    { $chance ->
        [1] { $deltasign ->
                [1] Agrega
                *[-1] Quita
            }
        *[other]
            { $deltasign ->
                [1] agregar
                *[-1] quitar
            }
    } {NATURALFIXED($amount, 2)}u de {$reagent} { $deltasign ->
        [1] a
        *[-1] de
    } la solución

entity-effect-guidebook-adjust-reagent-group =
    { $chance ->
        [1] { $deltasign ->
                [1] Agrega
                *[-1] Quita
            }
        *[other]
            { $deltasign ->
                [1] agregar
                *[-1] quitar
            }
    } {NATURALFIXED($amount, 2)}u de reactivos del grupo {$group} { $deltasign ->
            [1] a
            *[-1] de
        } la solución

entity-effect-guidebook-adjust-temperature =
    { $chance ->
        [1] { $deltasign ->
                [1] Agrega
                *[-1] Quita
            }
        *[other]
            { $deltasign ->
                [1] agregar
                *[-1] quitar
            }
    } {POWERJOULES($amount)} de calor { $deltasign ->
            [1] al
            *[-1] del
        } cuerpo en el que está

entity-effect-guidebook-chem-cause-disease =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } la enfermedad { $disease }

entity-effect-guidebook-chem-cause-random-disease =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } las enfermedades { $diseases }

entity-effect-guidebook-jittering =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } temblores

entity-effect-guidebook-clean-bloodstream =
    { $chance ->
        [1] Limpia
        *[other] limpiar
    } el torrente sanguíneo de otros químicos

entity-effect-guidebook-cure-disease =
    { $chance ->
        [1] Cura
        *[other] curar
    } enfermedades

entity-effect-guidebook-eye-damage =
    { $chance ->
        [1] { $deltasign ->
                [1] Inflige
                *[-1] Cura
            }
        *[other]
            { $deltasign ->
                [1] infligir
                *[-1] curar
            }
    } daño ocular

entity-effect-guidebook-vomit =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } vómitos

entity-effect-guidebook-create-gas =
    { $chance ->
        [1] Crea
        *[other] crear
    } { $moles } { $moles ->
        [1] mol
        *[other] moles
    } de { $gas }

entity-effect-guidebook-drunk =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } embriaguez

entity-effect-guidebook-electrocute =
    { $chance ->
        [1] { $stuns ->
            [true] Electrocuta
            *[false] Sacude con electricidad
            }
        *[other] { $stuns ->
            [true] electrocutar
            *[false] sacudir con electricidad
            }
    } al metabolizador durante {NATURALFIXED($time, 3)} segundos

entity-effect-guidebook-emote =
    { $chance ->
        [1] Obligará
        *[other] obligar
    } al metabolizador a [bold][color=white]{$emote}[/color][/bold]

entity-effect-guidebook-extinguish-reaction =
    { $chance ->
        [1] Apaga
        *[other] apagar
    } el fuego

entity-effect-guidebook-flammable-reaction =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } la inflamabilidad

entity-effect-guidebook-ignite =
    { $chance ->
        [1] Prende fuego
        *[other] prender fuego
    } al metabolizador

entity-effect-guidebook-make-sentient =
    { $chance ->
        [1] Vuelve
        *[other] volver
    } consciente al metabolizador

entity-effect-guidebook-make-polymorph =
    { $chance ->
        [1] Transforma
        *[other] transformar
    } al metabolizador en un { $entityname }

entity-effect-guidebook-modify-bleed-amount =
    { $chance ->
        [1] { $deltasign ->
                [1] Provoca
                *[-1] Reduce
            }
        *[other] { $deltasign ->
                    [1] provocar
                    *[-1] reducir
                 }
    } sangrado

entity-effect-guidebook-modify-blood-level =
    { $chance ->
        [1] { $deltasign ->
                [1] Aumenta
                *[-1] Disminuye
            }
        *[other] { $deltasign ->
                    [1] aumentar
                    *[-1] disminuir
                 }
    } el nivel de sangre

entity-effect-guidebook-paralyze =
    { $chance ->
        [1] Paraliza
        *[other] paralizar
    } al metabolizador durante al menos {NATURALFIXED($time, 3)} segundos

entity-effect-guidebook-movespeed-modifier =
    { $chance ->
        [1] Modifica
        *[other] modificar
    } la velocidad de movimiento en {NATURALFIXED($sprintspeed, 3)}x durante al menos {NATURALFIXED($time, 3)} segundos

entity-effect-guidebook-reset-narcolepsy =
    { $chance ->
        [1] Mantiene a raya temporalmente
        *[other] mantener a raya temporalmente
    } la narcolepsia

entity-effect-guidebook-wash-cream-pie-reaction =
    { $chance ->
        [1] Limpia
        *[other] limpiar
    } la tarta de crema de la cara

entity-effect-guidebook-cure-zombie-infection =
    { $chance ->
        [1] Cura
        *[other] curar
    } una infección zombi en curso

entity-effect-guidebook-cause-zombie-infection =
    { $chance ->
        [1] Contagia
        *[other] contagiar
    } a un individuo con la infección zombi

entity-effect-guidebook-innoculate-zombie-infection =
    { $chance ->
        [1] Cura
        *[other] curar
    } una infección zombi en curso, y otorga inmunidad ante infecciones futuras

entity-effect-guidebook-reduce-rotting =
    { $chance ->
        [1] Regenera
        *[other] regenerar
    } {NATURALFIXED($time, 3)} segundos de putrefacción

entity-effect-guidebook-area-reaction =
    { $chance ->
        [1] Provoca
        *[other] provocar
    } una reacción de humo o espuma durante {NATURALFIXED($duration, 3)} segundos

entity-effect-guidebook-add-to-solution-reaction =
    { $chance ->
        [1] Hace
        *[other] hacer
    } que se agregue {$reagent} a su contenedor de solución interno

entity-effect-guidebook-artifact-unlock =
    { $chance ->
        [1] Ayuda
        *[other] ayudar
        } a desbloquear un artefacto alienígena.

entity-effect-guidebook-artifact-durability-restore =
    Restaura {$restored} de durabilidad en los nodos activos de artefactos alienígenas.

entity-effect-guidebook-plant-attribute =
    { $chance ->
        [1] Ajusta
        *[other] ajustar
    } {$attribute} en {$positive ->
    [true] [color=red]{$amount}[/color]
    *[false] [color=green]{$amount}[/color]
    }

entity-effect-guidebook-plant-cryoxadone =
    { $chance ->
        [1] Rejuvenece
        *[other] rejuvenecer
    } la planta, según su edad y su tiempo de crecimiento

entity-effect-guidebook-plant-phalanximine =
    { $chance ->
        [1] Restaura
        *[other] restaurar
    } la viabilidad de una planta que una mutación volvió inviable

entity-effect-guidebook-plant-diethylamine =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } la longevidad y/o la salud base de la planta, con un 10% de probabilidad para cada una

entity-effect-guidebook-plant-robust-harvest =
    { $chance ->
        [1] Aumenta
        *[other] aumentar
    } la potencia de la planta en {$increase} hasta un máximo de {$limit}. Hace que la planta pierda sus semillas cuando la potencia llega a {$seedlesstreshold}. Intentar subir la potencia por encima de {$limit} puede reducir el rendimiento con un 10% de probabilidad

entity-effect-guidebook-plant-seeds-add =
    { $chance ->
        [1] Restaura las
        *[other] restaurar las
    } semillas de la planta

entity-effect-guidebook-plant-seeds-remove =
    { $chance ->
        [1] Quita las
        *[other] quitar las
    } semillas de la planta

entity-effect-guidebook-plant-mutate-chemicals =
    { $chance ->
        [1] Muta
        *[other] mutar
    } una planta para que produzca {$name}
