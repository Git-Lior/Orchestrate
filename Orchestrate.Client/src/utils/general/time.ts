import moment from "moment";

export function getTimeText(time: number) {
  return moment.unix(time).format("HH:mm");
}

export function getDateText(time: number) {
  return moment.unix(time).format("DD/MM/yyyy");
}

export function getFullTimeText(time: number) {
  return moment.unix(time).format("DD/MM/yyyy HH:mm:ss");
}

export function timeFromNow(time: number) {
  return moment.unix(time).fromNow(true);
}
