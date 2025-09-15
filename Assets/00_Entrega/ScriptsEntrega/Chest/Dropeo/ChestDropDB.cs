using UnityEngine;
using static ChestPressedLogic;

public static class ChestDropDB
{
    // Definición de ítem en código (no serializa, no se ve en el Inspector)
    public sealed class DropDef
    {
        public readonly string name;
        public readonly Rarity rarity;

        public DropDef(string name, Rarity rarity)
        {
            this.name = name;
            this.rarity = rarity;
        }
    }
    
    public static readonly DropDef[] All = new DropDef[]
    {
        new DropDef("Espada del Alba", Rarity.Normal),
        new DropDef("Daga de Sombras", Rarity.Normal),
        new DropDef("Hacha del Bosque Antiguo", Rarity.Normal),
        new DropDef("Maza de Hierro Viejo", Rarity.Normal),
        new DropDef("Lanza del Centinela", Rarity.Normal),
        new DropDef("Arco de Fresno", Rarity.Normal),
        new DropDef("Ballesta de Mercado", Rarity.Normal),
        new DropDef("Escudo de Roble", Rarity.Normal),
        new DropDef("Yelmo de Explorador", Rarity.Normal),
        new DropDef("Guanteletes Remachados", Rarity.Normal),
        new DropDef("Botas del Errante", Rarity.Normal),
        new DropDef("Armadura de Cuero Templado", Rarity.Normal),
        new DropDef("Amuleto de Cobre", Rarity.Normal),
        new DropDef("Anillo del Campesino", Rarity.Normal),
        new DropDef("Báculo de Novicio", Rarity.Normal),
        new DropDef("Varita Astillada", Rarity.Normal),
        new DropDef("Grimorio Polvoriento", Rarity.Normal),
        new DropDef("Orbe Opaco", Rarity.Normal),
        new DropDef("Talismán Desgastado", Rarity.Normal),
        new DropDef("Reliquia Oxidada", Rarity.Normal),
        new DropDef("Espada del Ocaso", Rarity.Normal),
        new DropDef("Hacha de Peltre", Rarity.Normal),
        new DropDef("Lanza de Guardia", Rarity.Normal),
        new DropDef("Arco de Saúco", Rarity.Normal),
        new DropDef("Escudo de Campaña", Rarity.Normal),
        new DropDef("Cota de Mallas Ligera", Rarity.Normal),
        new DropDef("Casco del Vigía", Rarity.Normal),
        new DropDef("Guantes de Caza", Rarity.Normal),
        new DropDef("Botas Reforzadas", Rarity.Normal),
        new DropDef("Medallón de Latón", Rarity.Normal),
        new DropDef("Anillo de Piedra Verde", Rarity.Normal),
        new DropDef("Cuchillo de Supervivencia", Rarity.Normal),
        new DropDef("Peto de Escamas", Rarity.Normal),
        new DropDef("Hombreras de Caza", Rarity.Normal),
        new DropDef("Cinturón con Hebilla", Rarity.Normal),
        new DropDef("Mandoble de Milicias", Rarity.Normal),
        new DropDef("Alabarda del Puerto", Rarity.Normal),
        new DropDef("Báculo de Corteza", Rarity.Normal),
        new DropDef("Arco Corto del Novato", Rarity.Normal),
        new DropDef("Trabuco Antiguo", Rarity.Normal),
        new DropDef("Mangual de Entrenamiento", Rarity.Normal),
        new DropDef("Lanza del Peregrino", Rarity.Normal),
        new DropDef("Arco de Hiedra", Rarity.Normal),
        new DropDef("Hacha de Risco", Rarity.Normal),
        new DropDef("Espada del Forjador", Rarity.Normal),
        new DropDef("Daga del Mercader", Rarity.Normal),
        new DropDef("Báculo del Roble Viejo", Rarity.Normal),
        new DropDef("Maza de la Muralla", Rarity.Normal),
        new DropDef("Hombreras del Centauro", Rarity.Normal),
        new DropDef("Mandoble Gris", Rarity.Normal),

        new DropDef("Espada Rúnica", Rarity.Rara),
        new DropDef("Daga del Venado Lunar", Rarity.Rara),
        new DropDef("Hacha de Obsidiana", Rarity.Rara),
        new DropDef("Lanza de Marfil Tallado", Rarity.Rara),
        new DropDef("Arco del Halcón", Rarity.Rara),
        new DropDef("Escudo del Guardabosques", Rarity.Rara),
        new DropDef("Yelmo Escamado del Norte", Rarity.Rara),
        new DropDef("Guanteletes de Sable", Rarity.Rara),
        new DropDef("Grebas del Jinete", Rarity.Rara),
        new DropDef("Armadura de Láminas Pulidas", Rarity.Rara),
        new DropDef("Amuleto de Ámbar Vivo", Rarity.Rara),
        new DropDef("Anillo del Susurro", Rarity.Rara),
        new DropDef("Báculo de Hielo Azul", Rarity.Rara),
        new DropDef("Varita de Chispa", Rarity.Rara),
        new DropDef("Grimorio de las Mareas", Rarity.Rara),
        new DropDef("Orbe del Eco", Rarity.Rara),
        new DropDef("Talismán del Vigía Nocturno", Rarity.Rara),
        new DropDef("Reliquia de los Primeros", Rarity.Rara),
        new DropDef("Mandoble del Lobo", Rarity.Rara),
        new DropDef("Hacha Doble de Tormenta", Rarity.Rara),
        new DropDef("Lanza Alada", Rarity.Rara),
        new DropDef("Arco del Ciervo Blanco", Rarity.Rara),
        new DropDef("Escudo de la Aurora", Rarity.Rara),
        new DropDef("Orbe Luctuoso", Rarity.Rara),
        new DropDef("Grebas de Tormenta", Rarity.Rara),
        new DropDef("Amuleto del Faro", Rarity.Rara),
        new DropDef("Anillo de Escarcha", Rarity.Rara),
        new DropDef("Talismán de Marea Alta", Rarity.Rara),
        new DropDef("Cota del Invierno", Rarity.Rara),
        new DropDef("Casco del Suspiro", Rarity.Rara),

        new DropDef("Espada Ígnea del Fénix", Rarity.Epica),
        new DropDef("Daga del Vacío", Rarity.Epica),
        new DropDef("Hacha del Coloso", Rarity.Epica),
        new DropDef("Lanza de Trueno", Rarity.Epica),
        new DropDef("Arco de las Estrellas", Rarity.Epica),
        new DropDef("Escudo del Guardián Eterno", Rarity.Epica),
        new DropDef("Corona del Archimago", Rarity.Epica),
        new DropDef("Guanteletes del Dragón", Rarity.Epica),
        new DropDef("Orbe de Cristal del Alba", Rarity.Epica),
        new DropDef("Grimorio de los Sellos", Rarity.Epica),
        new DropDef("Reliquia de Hueso de Dragón", Rarity.Epica),
        new DropDef("Escudo del Cazador de Sombras", Rarity.Epica),
        new DropDef("Grimorio del Cónclave", Rarity.Epica),
        new DropDef("Báculo de Jade Profundo", Rarity.Epica),
        new DropDef("Alabarda de la Aurora", Rarity.Epica),
        new DropDef("Varita del Faro Eterno", Rarity.Epica),

        new DropDef("Espada Legendaria Cantoceleste", Rarity.Legendaria),
        new DropDef("Báculo de Aurion, Rompelímites", Rarity.Legendaria),
        new DropDef("Escudo del Último Rey", Rarity.Legendaria),
        new DropDef("Orbe del Sol Ígneo", Rarity.Legendaria),

    };
}
