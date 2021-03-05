export function userToText(user: orch.UserData) {
  return `${user.firstName} ${user.lastName}`;
}

export function roleToText(role: orch.Role) {
  return `${role.section}${!role.num ? "" : " " + role.num}`;
}
