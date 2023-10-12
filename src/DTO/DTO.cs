using System;
using System.Runtime.InteropServices;
using Radical.Servitizing.Entity.Identifier;

namespace Radical.Servitizing.DTO;

using Uniques;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public class DTO : UniqueIdentifiable, IDTO
{
}