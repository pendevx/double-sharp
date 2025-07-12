using Music.Models.Data;

namespace Music.Global.Contracts;

public delegate bool RequiresPermission(params RoleName[] roles);
