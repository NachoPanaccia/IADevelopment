// Archivo: IFlockingComportamiento.cs
using System.Collections.Generic;
using UnityEngine;

public interface IFlockingComportamiento
{
    
    Vector3 ObtenerDireccion(List<BoidLobo> todos);
}
