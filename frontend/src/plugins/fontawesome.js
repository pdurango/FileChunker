import Vue from "vue";

import { library } from "@fortawesome/fontawesome-svg-core";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";
import
{
    faUserSecret,
    faHome,
    faBriefcase,
    faPlay,
    faStop,
    faEdit,
    faCog,
    faInfoCircle,
    faChevronRight,
    faChevronLeft,
    faPlusSquare,
    faCalendarDay,
    faClock,
    faTrashAlt,
    faUpload,
    faPlus,
    faTasks,
    faCheck,
} from "@fortawesome/free-solid-svg-icons";

library.add(faUserSecret);
library.add(faHome);
library.add(faBriefcase);
library.add(faPlay);
library.add(faStop);
library.add(faEdit);
library.add(faCog);
library.add(faInfoCircle);
library.add(faChevronRight);
library.add(faChevronLeft);
library.add(faPlusSquare);
library.add(faCalendarDay);
library.add(faClock);
library.add(faTrashAlt);
library.add(faUpload);
library.add(faPlus);
library.add(faTasks);
library.add(faCheck);

Vue.component("font-awesome-icon", FontAwesomeIcon);
