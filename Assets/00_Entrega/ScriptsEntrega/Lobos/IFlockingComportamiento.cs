// Archivo: IFlockingComportamiento.cs
using System.Collections.Generic;
using UnityEngine;

public interface IFlockingComportamiento
{
    // Devuelve un vector de steering que este boid aplicará.
    Vector3 ObtenerDireccion(List<BoidLobo> todos);
}
