## Terminal Seguro de Comando - interfaz

secure-terminal-window-title = Terminal Seguro
secure-terminal-requests-header = Solicitudes
secure-terminal-information-header = Información
secure-terminal-authorization-header = Autorización
secure-terminal-select-request = Seleccioná una solicitud de la lista de la izquierda para ver sus detalles.
secure-terminal-request-button = Solicitar
secure-terminal-request-button-confirm = ¿Confirmar?
secure-terminal-authorize-button = Autorizar
secure-terminal-deny-button = Denegar / Cancelar
secure-terminal-recall-button = Retirar armería
secure-terminal-recall-locked = { $minutes ->
    [1] El retiro estará disponible en 1 minuto.
   *[other] El retiro estará disponible en {$minutes} minutos.
}
secure-terminal-used-note = Esta armería ya fue activada o retirada definitivamente durante la ronda y no puede volver a desplegarse.
secure-terminal-already-used = Este recurso ya fue utilizado durante la ronda y no puede volver a solicitarse.
secure-terminal-auth-waiting = No hay una propuesta activa para esta solicitud.
secure-terminal-auth-desc = Propuesta actual: sin respuesta = [color=red]rojo[/color], aceptada = [color=green]verde[/color]:
secure-terminal-awaiting-member = Esperando a {$label}
secure-terminal-pending-countdown-label = Expira en {$minutes}m {$seconds}s...
secure-terminal-countdown-label = Se activa en {$minutes}m {$seconds}s...
secure-terminal-fee-note = Costo de procesamiento: {$fee}
secure-terminal-salary-note = El salario de la estación se reduce un {$penalty}% por el costo de movilización.
secure-terminal-delay-note = { $minutes ->
    [1] Tiempo estimado: 1 minuto después de la autorización.
   *[other] Tiempo estimado: {$minutes} minutos después de la autorización.
}
secure-terminal-requires-no-war-note = Deshabilitado durante Operaciones de Guerra.
secure-terminal-requires-war-note = Disponible solamente durante Operaciones de Guerra.
secure-terminal-requires-alert-note = Requiere que la alerta {$level} esté activa.
secure-terminal-alert-time-remaining = { $minutes ->
    [1] La alerta debe permanecer activa 1 minuto más antes de poder solicitar esto.
   *[other] La alerta debe permanecer activa {$minutes} minutos más antes de poder solicitar esto.
}
secure-terminal-on-cooldown-note = { $minutes ->
    [1] En espera: disponible en 1 minuto.
   *[other] En espera: disponible en {$minutes} minutos.
}
secure-terminal-requires-alert-suffix = Requiere: {$level}
secure-terminal-requires-war-suffix = Requiere: Operaciones de Guerra
secure-terminal-reason = Ingresá el motivo de la solicitud:

## Anuncios globales

secure-terminal-proposal-created = Se solicitó {$request}; está esperando autorización conjunta.
secure-terminal-proposal-created-reason = Se solicitó {$request}; está esperando autorización conjunta. Motivo: {$reason}
secure-terminal-proposal-denied = Se canceló la solicitud de {$request}.
secure-terminal-proposal-denied-cc = Comando Central denegó la solicitud de {$request}.
secure-terminal-radio-proposal = Se propuso {$request}. Diríjanse al dispositivo de autenticación con tarjeta más cercano para autorizarla o denegarla.
secure-terminal-radio-proposal-reason = Se propuso {$request}. Diríjanse al dispositivo de autenticación con tarjeta más cercano para autorizarla o denegarla. Motivo: {$reason}
secure-terminal-radio-denied = Se canceló la solicitud de {$request}.
secure-terminal-activation-countdown = {$request} recibió todas las autorizaciones.
    Se activará en {$minutes} minutos.
    El salario de la estación se redujo por el costo de movilización.
secure-terminal-unknown-job = Desconocido

## Mensajes emergentes

secure-terminal-no-station = No se encontró una estación para esta consola.
secure-terminal-request-denied = Acceso denegado.
secure-terminal-authorize-denied = No tenés la autorización requerida para firmar esta solicitud.
secure-terminal-requires-war = Esta solicitud sólo está disponible cuando se declararon formalmente Operaciones de Guerra.
secure-terminal-wrong-alert = El nivel de alerta actual no cumple los requisitos de esta solicitud.
secure-terminal-alert-not-long-enough = El nivel de alerta no lleva suficiente tiempo activo. Esperá e intentá nuevamente.
secure-terminal-recall-too-soon = La armería no lleva suficiente tiempo desplegada para retirarla. Esperá.
secure-terminal-on-cooldown = Esta solicitud está en espera.
secure-terminal-already-pending = Ya hay una propuesta pendiente para esta solicitud.
secure-terminal-already-active = Ya hay otra solicitud pendiente o activándose. Esperá a que termine antes de crear una nueva.
secure-terminal-no-active-proposal = No se encontró una propuesta activa para esta solicitud.
secure-terminal-already-authorized = Ya autorizaste esta propuesta.
secure-terminal-already-activated = Esta terminal ya autorizó la propuesta.
secure-terminal-auth-note = Esta terminal sirve únicamente para autorizar.
secure-terminal-authorized-by = Atención: se autorizó la solicitud de {$request}. Firmantes: {$signatories}.
secure-terminal-armory-recalled = Se emitió la orden de retiro de {$request}. El despliegue de la armería fue cancelado.
secure-terminal-awaiting-admin = Atención: se envió la solicitud de {$request}. Esperando autorización de Comando Central.
secure-terminal-admin = Se solicita aprobación administrativa para: {$request}
                        Motivo: {$reason}
                        Usá AGhost para aprobar o denegar la solicitud.

## Nombres y descripciones

secure-terminal-warops-security-name = Equipo de Respuesta Nuclear
secure-terminal-warops-security-desc = Despliega una unidad ERT de Seguridad especializada en Operaciones de Guerra. Sólo está disponible durante Operaciones de Guerra.
                                       Usala cuando la estación esté bajo un asalto armado directo durante una declaración de guerra.
secure-terminal-warops-security-announcement = Se autorizó un Equipo de Respuesta a Emergencias especializado en Seguridad y está en camino. Tiempo estimado de llegada: 30 minutos.
secure-terminal-ert-security-name = ERT de Seguridad
secure-terminal-ert-security-desc = Despliega una unidad ERT de Seguridad.
secure-terminal-ert-security-announcement = Se autorizó un Equipo de Respuesta a Emergencias de Seguridad y está en camino. Tiempo estimado de llegada: 10 minutos.
secure-terminal-ert-engineering-name = ERT de Ingeniería
secure-terminal-ert-engineering-desc = Despliega una unidad ERT de Ingeniería para asistir con infraestructura crítica de la estación.
    Se recomienda cuando la estación sufrió fallas estructurales, atmosféricas o eléctricas catastróficas que exceden la capacidad de reparación local.
secure-terminal-ert-engineering-announcement = Se autorizó un Equipo de Respuesta a Emergencias de Ingeniería y está en camino. Tiempo estimado de llegada: 10 minutos.
secure-terminal-ert-medical-name = ERT Médica
secure-terminal-ert-medical-desc = Despliega una unidad ERT Médica para triaje masivo y cirugía de emergencia.
    Se recomienda cuando el departamento médico está desbordado, incapacitado o destruido.
secure-terminal-ert-medical-announcement = Se autorizó un Equipo de Respuesta a Emergencias Médicas y está en camino. Tiempo estimado de llegada: 10 minutos.
secure-terminal-ert-janitorial-name = ERT de Limpieza
secure-terminal-ert-janitorial-desc = Despliega una unidad ERT de Limpieza para descontaminación peligrosa y restauración de la estación.
    Se recomienda después de contaminación biológica, química o ambiental a gran escala que requiera limpieza inmediata.
secure-terminal-ert-janitorial-announcement = Se autorizó un Equipo de Respuesta a Emergencias de Limpieza y está en camino. Tiempo estimado de llegada: 10 minutos.
secure-terminal-ert-chaplain-name = ERT de Capellanía
secure-terminal-ert-chaplain-desc = Despliega un capellán ERT para apoyar la moral de la tripulación y administrar los últimos ritos.
    Brinda apoyo pastoral y sostiene la moral durante emergencias prolongadas.
secure-terminal-ert-chaplain-announcement = Se autorizó un Equipo de Respuesta a Emergencias de Capellanía y está en camino. Tiempo estimado de llegada: 10 minutos.
secure-terminal-ert-cburn-name = ERT CBURN
secure-terminal-ert-cburn-desc = Despliega una unidad ERT CBURN.
secure-terminal-ert-cburn-announcement = Se autorizó un Equipo de Respuesta a Emergencias CBURN y está en camino. Tiempo estimado de llegada: 15 minutos.
secure-terminal-code-gamma-name = Código GAMMA
secure-terminal-code-gamma-desc = Eleva la estación a alerta [color=palevioletred]GAMMA[/color]. Se declara la ley marcial; Seguridad debe escoltar a todos los civiles hacia zonas seguras.
    Seguridad debe permanecer armada. Los civiles deben presentarse ante el jefe de personal más cercano para ser escoltados a un lugar seguro. Se activan las luces de emergencia.
secure-terminal-code-gamma-announcement = ¡Atención! El Código GAMMA entrará en vigor en breve. Se aplicará la ley marcial. Toda la tripulación debe presentarse inmediatamente ante el jefe de personal más cercano.
secure-terminal-end-gamma-name = Finalizar alerta GAMMA
secure-terminal-end-gamma-desc = Levanta la alerta [color=palevioletred]GAMMA[/color] y devuelve la estación a Verde. GAMMA debe haber estado activa al menos 15 minutos.
secure-terminal-end-gamma-announcement = Se levanta el Código GAMMA. La estación vuelve a sus operaciones normales. Manténganse alerta y esperen instrucciones de su jefe de personal.
secure-terminal-code-psi-name = Código PSI
secure-terminal-code-psi-desc = Eleva la estación a alerta [color=mediumpurple]PSI[/color]. Se detectaron unidades sintéticas hostiles; eviten cíborgs no conformes y busquen al personal de mando.
    Indica actividad cíborg hostil o no conforme. La tripulación debe evitar borgs desconocidos, permanecer en grupos y seguir las indicaciones de los jefes de personal.
secure-terminal-code-psi-announcement = ¡Atención! Comando autorizó el Código PSI. Se identificaron unidades de silicio ajenas a NanoTrasen como amenaza activa. Toda la tripulación debe presentarse ante el jefe de personal más cercano.
secure-terminal-end-psi-name = Finalizar alerta PSI
secure-terminal-end-psi-desc = Levanta la alerta [color=mediumpurple]PSI[/color] y devuelve la estación a Verde. PSI debe haber estado activa al menos 15 minutos.
secure-terminal-end-psi-announcement = Se levanta el Código PSI. La amenaza sintética identificada fue neutralizada. La estación vuelve a sus operaciones normales.
secure-terminal-armory-gamma-name = Armería Gamma
secure-terminal-armory-gamma-desc = Despacha la [color=palevioletred]Armería Gamma[/color], un depósito de armamento pesado para situaciones GAMMA. Se despliega una sola vez.
                                    Entrega equipamiento pesado de seguridad al personal autorizado.
secure-terminal-armory-gamma-announcement = Se autorizó la Armería Gamma y está en camino.
secure-terminal-armory-psi-name = Armería Psi
secure-terminal-armory-psi-desc = Despacha la [color=mediumpurple]Armería Psi[/color], con armamento anticibernético para situaciones PSI. Se despliega una sola vez.
                                  Proporciona herramientas para neutralizar unidades de silicio no conformes.
secure-terminal-armory-psi-announcement = Se autorizó la Armería Psi y está en camino.
secure-terminal-med-pod-name = Cápsula Médica de Emergencia
secure-terminal-med-pod-desc = Despacha la Cápsula Médica de Emergencia, una unidad rápida de triaje con equipamiento quirúrgico y de reanimación.
    Usala cuando las bajas masivas excedan la capacidad médica de la estación.
secure-terminal-med-pod-announcement = Se autorizó la Cápsula Médica de Emergencia y está en camino. Tiempo estimado de llegada: 5 minutos.
secure-terminal-nukerequest-name = Código de autodestrucción
secure-terminal-nukerequest-desc = Solicita los códigos nucleares de autodestrucción.
                                   El uso indebido del sistema de solicitud nuclear no será tolerado bajo ninguna circunstancia.
                                   La transmisión no garantiza una respuesta.
secure-terminal-code-violet-name = Código Violeta
secure-terminal-code-violet-desc = Eleva la estación a alerta [color=Violet]Violeta[/color].
secure-terminal-end-violet-name = Finalizar alerta Violeta
secure-terminal-end-violet-desc = Levanta la alerta [color=Violet]Violeta[/color] y devuelve la estación a Verde. Violeta debe haber estado activa al menos 10 minutos.
secure-terminal-emergency-maintenance-name = Acceso de emergencia a mantenimiento
secure-terminal-emergency-maintenance-desc = Habilita el acceso de emergencia a mantenimiento.
secure-terminal-emergency-maintenance-announcement = Se eliminaron las restricciones de acceso a mantenimiento y a las esclusas exteriores.
secure-terminal-end-emergency-maintenance-name = Revocar acceso de emergencia a mantenimiento
secure-terminal-end-emergency-maintenance-desc = Revoca el acceso de emergencia a mantenimiento.
secure-terminal-end-emergency-maintenance-announcement = Se restauraron las restricciones de acceso a mantenimiento y a las esclusas exteriores.
secure-terminal-emergency-station-name = Acceso de emergencia para toda la estación
secure-terminal-emergency-station-desc = Activa el acceso de emergencia en toda la estación.
secure-terminal-emergency-station-announcement = Se eliminaron las restricciones de acceso de todas las esclusas debido a la crisis en curso. Las leyes de intrusión continúan vigentes salvo orden contraria del personal de mando.
secure-terminal-end-emergency-station-name = Desactivar acceso de emergencia para toda la estación
secure-terminal-end-emergency-station-desc = Desactiva el acceso de emergencia en toda la estación.
secure-terminal-end-emergency-station-announcement = Se restauraron las restricciones de acceso de todas las esclusas. Si quedaron atrapados, soliciten ayuda a la IA de la estación o a un colega.
