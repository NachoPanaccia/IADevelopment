// Archivo: IFlockingComportamiento.cs
using System.Collections.Generic;
using UnityEngine;

public interface IFlockingComportamiento
{
    // Debe devolver un vector (sin aplicar Time.deltaTime)
    Vector3 ObtenerDireccion(List<BoidLobo> todos);
}