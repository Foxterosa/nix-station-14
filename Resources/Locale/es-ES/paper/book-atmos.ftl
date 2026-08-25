book-text-atmos-distro = La red de distribución, o "distro" para abreviar, es la línea de vida de la estación. Se encarga de transportar aire desde atmosférica a toda la estación.

        Las tuberías relevantes suelen estar pintadas de azul apagado, pero una forma segura de identificarlas es usar un escáner de bandeja para rastrear qué tuberías están conectadas a las ventilas activas de la estación.

        La mezcla estándar de gases de la red de distribución es de 20 grados celsius, 78 % nitrógeno y 22 % oxígeno. Puedes comprobarlo usando un analizador de gases en una tubería de distro o en cualquier ventila conectada a ella. Circunstancias especiales pueden requerir mezclas especiales.

        A la hora de decidir la presión de la distro, hay varias cosas a considerar. Las ventilas activas regulan la presión de la estación, así que, mientras todo funcione correctamente, no existe una presión de distro "demasiado alta".

        Una presión de distro más alta permitirá que la red de distro actúe como amortiguador entre los mineros de gas y las ventilas, proporcionando una cantidad significativa de aire extra que puede usarse para volver a presurizar la estación tras una apertura al espacio.

        Una presión de distro más baja reducirá la cantidad de gas perdido en caso de que la distro quede expuesta al espacio, una forma rápida de lidiar con la contaminación de la distro. También puede ayudar a ralentizar o evitar la sobrepresurización de la estación si hay problemas con las ventilas.

        Las presiones comunes de distro están en el rango de 300-375 kPa, pero se pueden usar otras presiones si se conocen sus riesgos y beneficios.

        La presión de la red está determinada por la última bomba que bombea hacia ella. Para evitar cuellos de botella, todas las demás bombas entre los mineros y la última bomba deben configurarse a su velocidad máxima, y cualquier dispositivo innecesario debe retirarse.

        Puedes validar la presión de la distro con un analizador de gases, pero ten en cuenta que una demanda alta debido a cosas como aperturas al espacio puede hacer que la distro se mantenga por debajo de la presión objetivo establecida durante períodos prolongados. Así que, si ves una caída de presión, no entres en pánico: puede ser temporal.

book-text-atmos-waste = La red de desechos es el sistema principal encargado de mantener el aire de la estación libre de contaminantes.

        Puedes identificar las tuberías relevantes por su color rojo apagado o usando un escáner de bandeja para rastrear qué tuberías están conectadas a los depuradores de la estación.

        La red de desechos se utiliza para transportar gases residuales para filtrarlos o expulsarlos al espacio. Lo ideal es mantener la presión en 0 kPa, aunque a veces puede estar en una presión baja distinta de cero mientras está en uso.

        Los técnicos tienen la opción de filtrar o expulsar al espacio los gases residuales. Aunque expulsarlos al espacio es más rápido, filtrarlos permite reutilizarlos para reciclarlos o venderlos.

        La red de desechos también puede usarse para diagnosticar problemas atmosféricos en la estación. Niveles altos de un gas residual pueden sugerir una fuga grande, mientras que la presencia de gases que no son de desecho puede indicar un problema de configuración o de conexión física de un depurador. Si los gases están a alta temperatura, podría indicar un incendio.

book-text-atmos-alarms = Las alarmas de aire están distribuidas por toda la estación para permitir la gestión y el monitoreo de la atmósfera local.

            La interfaz de la alarma de aire proporciona a los técnicos una lista de sensores conectados, sus lecturas y la posibilidad de ajustar umbrales. Estos umbrales se usan para determinar la condición de alarma de la alarma de aire. Los técnicos también pueden usar la interfaz para establecer presiones objetivo para las ventilas y configurar las velocidades de funcionamiento y los gases objetivo de los depuradores.

            Aunque la interfaz permite ajustar con precisión los dispositivos bajo el control de la alarma de aire, también hay varios modos disponibles para configurar rápidamente la alarma. Estos modos se activan automáticamente cuando cambia el estado de la alarma:
            - Filtrado: el modo predeterminado
            - Filtrado (amplio): un modo de filtrado que modifica el funcionamiento de los depuradores para cubrir un área mayor
            - Llenado: desactiva los depuradores y ajusta las ventilas a su presión máxima
            - Pánico: desactiva las ventilas y pone a los depuradores en sifonado

            Se puede usar una multiherramienta o un configurador de red para enlazar dispositivos a las alarmas de aire.

book-text-atmos-vents =
    A continuación hay una guía de referencia rápida sobre varios dispositivos atmosféricos:

                Ventilas pasivas:
                Estas ventilas no requieren energía; permiten que los gases fluyan libremente tanto hacia dentro como hacia fuera de la red de tuberías a la que están conectadas.

                Ventilas activas:
                Estas son las ventilas más comunes de la estación. Tienen una bomba interna y requieren energía. Por defecto, solo bombean gases fuera de las tuberías y solo hasta 101 kPa. Sin embargo, pueden reconfigurarse usando una alarma de aire. También se bloquean si la sala está por debajo de 1 kPa, para evitar bombear gases al espacio.

                Depuradores de aire:
                Estos dispositivos permiten extraer gases del ambiente y llevarlos a la red de tuberías conectada. Se pueden configurar para seleccionar gases específicos cuando están conectados a una alarma de aire.

                Inyectores de aire:
                Los inyectores son parecidos a las ventilas activas, pero no tienen bomba interna ni requieren energía. No se pueden configurar, pero pueden seguir bombeando gases hasta presiones mucho más altas.
