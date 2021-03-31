import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

import React, { useCallback, useMemo } from "react";
import Slider from "react-slick";

import { Breakpoint } from "@material-ui/core/styles/createBreakpoints";
import { lighten, makeStyles } from "@material-ui/core/styles";
import Typography from "@material-ui/core/Typography";
import Container from "@material-ui/core/Container";
import ChevronLeftIcon from "@material-ui/icons/ChevronLeft";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import DeleteIcon from "@material-ui/icons/DeleteOutline";
import TextField from "@material-ui/core/TextField";

import { UsersListInput } from "utils/components";
import { useApiFetch, useInputState, useWidth } from "utils/hooks";

interface Props extends Required<orch.PageProps> {
  getAllUsers: () => Promise<orch.UserData[]>;
}

const SLIDER_BREAKPOINTS: Record<Breakpoint, number> = {
  xs: 1,
  sm: 1,
  md: 2,
  lg: 3,
  xl: 4,
};

const useStyles = makeStyles(theme => ({
  rolesSlider: {
    "&, & .slick-list, & .slick-track, & .slick-slide": { height: "100%" },
    "& .slick-slide": { display: "flex", flexDirection: "column", "& > div": { flex: 1 } },
    "& .slick-prev": { left: -30 },
    "& .slick-next": { right: -30 },
    "& .slick-prev, & .slick-next": {
      "&.slick-disabled": { display: "none !important" },
      "&:hover": { color: lighten(theme.palette.primary.main, 0.3) },
    },
  },
  roleContainer: {
    width: "100%",
    height: "100%",
    padding: "2rem",
    display: "inline-flex !important",
    flexDirection: "column",
    "& > *": { width: "100%" },
  },
  roleHeader: {
    marginBottom: "1rem",
    display: "flex",
    justifyContent: "center",
    "& :not(:last-child)": { marginRight: "1rem" },
  },
  roleTitle: { fontWeight: "bold" },
  roleFilter: {
    position: "absolute",
    top: "3rem",
    left: "4rem",
  },
}));

export default function RolesPanel({ user, group, userInfo, setGroup, getAllUsers }: Props) {
  const classes = useStyles();
  const width = useWidth();
  const apiFetch = useApiFetch(user, `/groups/${group.id}/roles`);
  const [roleFilter, setRoleFilter] = useInputState();

  const filteredRoles = useMemo(() => {
    return group.roles.filter(({ section, num }) => {
      const roleText = !num ? section : `${section} ${num}`;
      return roleText.toLowerCase().includes(roleFilter.toLowerCase());
    });
  }, [group.roles, roleFilter]);

  const removeRole = useCallback(
    (roleId: number) => {
      apiFetch(roleId.toString(), { method: "DELETE" }, "none").then(() =>
        setGroup(group => ({ ...group!, roles: group!.roles.filter(_ => _.id !== roleId) }))
      );
    },
    [setGroup, apiFetch]
  );

  const addMember = useCallback(
    (roleId: number, member: orch.UserData) => {
      return apiFetch(
        `${roleId}/members`,
        {
          method: "POST",
          body: JSON.stringify(member.id),
        },
        "none"
      ).then(() => {
        setGroup(group => {
          const roles = [...group!.roles];
          const roleIndex = roles.findIndex(_ => _.id === roleId);
          roles[roleIndex] = {
            ...roles[roleIndex],
            members: [...roles[roleIndex].members, member],
          };

          return { ...group!, roles };
        });
      });
    },
    [setGroup, apiFetch]
  );

  const removeMember = useCallback(
    (roleId: number, member: orch.UserData) => {
      return apiFetch(`${roleId}/members/${member.id}`, { method: "DELETE" }, "none").then(() => {
        setGroup(group => {
          const roles = [...group!.roles];
          const roleIndex = roles.findIndex(_ => _.id === roleId);
          roles[roleIndex] = {
            ...roles[roleIndex],
            members: roles[roleIndex].members.filter(_ => _.id !== member.id),
          };

          return { ...group!, roles };
        });
      });
    },
    [setGroup, apiFetch]
  );

  return (
    <>
      <TextField
        className={classes.roleFilter}
        placeholder="Filter Roles..."
        value={roleFilter}
        onChange={setRoleFilter}
      />
      <Slider
        className={classes.rolesSlider}
        arrows
        infinite={false}
        draggable={false}
        nextArrow={<ChevronRightIcon color="primary" fontSize="large" />}
        prevArrow={<ChevronLeftIcon color="primary" fontSize="large" />}
        speed={500}
        rows={2}
        slidesPerRow={SLIDER_BREAKPOINTS[width]}
      >
        {filteredRoles.map(({ id, section, num, members }) => (
          <Container key={id} maxWidth="sm" disableGutters className={classes.roleContainer}>
            <div className={classes.roleHeader}>
              <Typography variant="body1" className={classes.roleTitle}>
                {section} {num || undefined}
              </Typography>
              {userInfo.manager && (
                <DeleteIcon color="primary" cursor="pointer" onClick={() => removeRole(id)} />
              )}
            </div>
            <UsersListInput
              readonly={!userInfo.manager}
              users={members}
              optionsProvider={getAllUsers}
              onAdded={user => addMember(id, user)}
              onRemoved={user => removeMember(id, user)}
            />
          </Container>
        ))}
      </Slider>
    </>
  );
}
