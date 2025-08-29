let startDat = null;
let endDat = null;
let rangesData = null;
let total = 0;
function CalendarControl() {
    let calendar = new Date();
    const calendarControl = {
        localDate: new Date(),
        prevMonthLastDate: null,
        calWeekDays: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
        calMonthName: [
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec"
        ],
        AddRangesData: function (data, tt) {
            rangesData = data;
            total = tt;
            calendarControl.attachEventsOnNextPrev();
        },
        highlightRange: function () {
            //check rangesData is not empty
            if (!rangesData) return;
            // Lấy tất cả các phần tử có class 'calendar'
            const calendars = document.querySelectorAll('.calendar');

            calendars.forEach((Icalendar) => {
                // Lấy danh sách các ngày trong lịch (giả sử có class 'number-item')
                const dateElements = Icalendar.querySelectorAll('.number-item');
                const today = new Date().toDateString();

                const ranges = rangesData; // Chuyển đổi chuỗi JSON thành đối tượng

                // Duyệt qua từng ngày trong lịch hiện tại
                dateElements.forEach((dayElement) => {
                    const dayNumber = parseInt(dayElement.dataset.num);
                    const date = new Date(calendar.getFullYear(), calendar.getMonth(), dayNumber);
                    date.setHours(0, 0, 0, 0);
                    let isInRange = false;  // Cờ để theo dõi xem ngày có nằm trong phạm vi hay không
                    let soLuongCount = 0;
                    // Kiểm tra xem ngày có nằm trong khoảng bắt đầu và kết thúc của ranges
                    ranges.forEach((range) => {
                        const startDate = new Date(range.start);
                        const endDate = new Date(range.end);
                        startDate.setHours(0, 0, 0, 0);
                        endDate.setHours(0, 0, 0, 0);
                        // Nếu ngày nằm trong khoảng, tô màu
                        if (date >= startDate && date <= endDate) {
                            isInRange = true;  // Đánh dấu ngày này nằm trong phạm vi
                            soLuongCount += parseInt(range.soluong);
                        }
                    });
                    if (isInRange) {
                        if (date.toDateString() !== today) {  // Không highlight cho ngày hôm nay
                            dayElement.classList.add('highlight-range');
                        }
                    }
                    dayElement.innerHTML += "còn " + (total - soLuongCount) + " thiết bị";
                });
            });
        },
        daysInMonth: function (month, year) {
            return new Date(year, month, 0).getDate();
        },
        firstDay: function () {
            return new Date(calendar.getFullYear(), calendar.getMonth(), 1);
        },
        lastDay: function () {
            return new Date(calendar.getFullYear(), calendar.getMonth() + 1, 0);
        },
        firstDayNumber: function () {
            return calendarControl.firstDay().getDay() + 1;
        },
        lastDayNumber: function () {
            return calendarControl.lastDay().getDay() + 1;
        },
        getPreviousMonthLastDate: function () {
            let lastDate = new Date(
                calendar.getFullYear(),
                calendar.getMonth(),
                0
            ).getDate();
            return lastDate;
        },
        navigateToPreviousMonth: function () {
            calendar.setMonth(calendar.getMonth() - 1);
            calendarControl.attachEventsOnNextPrev();
        },
        navigateToNextMonth: function () {
            calendar.setMonth(calendar.getMonth() + 1);
            calendarControl.attachEventsOnNextPrev();
        },
        navigateToCurrentMonth: function () {
            let currentMonth = calendarControl.localDate.getMonth();
            let currentYear = calendarControl.localDate.getFullYear();
            calendar.setMonth(currentMonth);
            calendar.setYear(currentYear);
            calendarControl.attachEventsOnNextPrev();
        },
        displayYear: function () {
            let yearLabel = document.querySelector(".calendar .calendar-year-label");
            yearLabel.innerHTML = calendar.getFullYear();
        },
        displayMonth: function () {
            let monthLabel = document.querySelector(
                ".calendar .calendar-month-label"
            );
            monthLabel.innerHTML = calendarControl.calMonthName[calendar.getMonth()];
        },
        selectDate: function (e) {
            console.log(
                `${e.target.textContent} ${calendarControl.calMonthName[calendar.getMonth()]
                } ${calendar.getFullYear()}`
            );
        },
        plotSelectors: function () {
            document.querySelector(
                ".calendar"
            ).innerHTML += `<div class="calendar-inner"><div class="calendar-controls">
          <div class="calendar-prev"><a href="#"><svg xmlns="http://www.w3.org/2000/svg" width="128" height="128" viewBox="0 0 128 128"><path fill="#666" d="M88.2 3.8L35.8 56.23 28 64l7.8 7.78 52.4 52.4 9.78-7.76L45.58 64l52.4-52.4z"/></svg></a></div>
          <div class="calendar-year-month">
          <div class="calendar-month-label"></div>
          <div>-</div>
          <div class="calendar-year-label"></div>
          </div>
          <div class="calendar-next"><a href="#"><svg xmlns="http://www.w3.org/2000/svg" width="128" height="128" viewBox="0 0 128 128"><path fill="#666" d="M38.8 124.2l52.4-52.42L99 64l-7.77-7.78-52.4-52.4-9.8 7.77L81.44 64 29 116.42z"/></svg></a></div>
          </div>
          <div class="calendar-today-date">Today: 
            ${calendarControl.calWeekDays[calendarControl.localDate.getDay()]}, 
            ${calendarControl.localDate.getDate()}, 
            ${calendarControl.calMonthName[calendarControl.localDate.getMonth()]} 
            ${calendarControl.localDate.getFullYear()}
          </div>
          <div class="calendar-body"></div></div>`;
        },
        plotDayNames: function () {
            for (let i = 0; i < calendarControl.calWeekDays.length; i++) {
                document.querySelector(
                    ".calendar .calendar-body"
                ).innerHTML += `<div>${calendarControl.calWeekDays[i]}</div>`;
            }
        },
        plotDates: function () {
            startDat = null;
            endDat = null;
            document.querySelector(".calendar .calendar-body").innerHTML = "";
            calendarControl.plotDayNames();
            calendarControl.displayMonth();
            calendarControl.displayYear();
            let count = 1;
            let prevDateCount = 0;

            calendarControl.prevMonthLastDate = calendarControl.getPreviousMonthLastDate();
            let prevMonthDatesArray = [];
            let calendarDays = calendarControl.daysInMonth(
                calendar.getMonth() + 1,
                calendar.getFullYear()
            );
            // dates of current month
            for (let i = 1; i < calendarDays; i++) {
                if (i < calendarControl.firstDayNumber()) {
                    prevDateCount += 1;
                    document.querySelector(
                        ".calendar .calendar-body"
                    ).innerHTML += `<div class="prev-dates"></div>`;
                    prevMonthDatesArray.push(calendarControl.prevMonthLastDate--);
                } else {
                    document.querySelector(
                        ".calendar .calendar-body"
                    ).innerHTML += `<div class="number-item" data-num=${count}><a class="dateNumber" onclick = "CalendarControl().OnclickAddevent(this)">${count++}</a></div>`;
                }
            }
            //remaining dates after month dates
            for (let j = 0; j < prevDateCount + 1; j++) {
                document.querySelector(
                    ".calendar .calendar-body"
                ).innerHTML += `<div class="number-item" data-num=${count}><a class="dateNumber" onclick = "CalendarControl().OnclickAddevent(this)">${count++}</a></div>`;
            }
            document.querySelector(".calendar").innerHTML +='<div id="error-message" style="color: red; display: none;"></div>';
            calendarControl.highlightToday();
            calendarControl.highlightRange();
            calendarControl.plotPrevMonthDates(prevMonthDatesArray);
            calendarControl.plotNextMonthDates();
        },
        OnclickAddevent: function (dayElement) {
            var errorMessage = document.getElementById('error-message');
            if (dayElement === startDat) {
                calendarControl.HighlightSelectedRange(false);
                startDat = null;
                endDat = null;
            } else if (!calendarControl.CheckInRange(dayElement)) {
                errorMessage.innerText = "Không thể chọn ngày này.";
                errorMessage.style.display = "block";
            } else {
                errorMessage.innerText = "";
                errorMessage.style.display = "none";
                calendarControl.HighlightSelectedRange(false);
                endDat = startDat;
                startDat = dayElement;
                calendarControl.HighlightSelectedRange(true);
            }
        },
        CheckInRange: function (dayElement) {
            if (dayElement.parentElement.classList.contains("highlight-range")) return false;
            if (startDat == null) return true;
            let dateNumber = document.querySelectorAll(".calendar .dateNumber");
            let start = parseInt(startDat.parentElement.getAttribute('data-num'));
            let end = parseInt(dayElement.parentElement.getAttribute('data-num'));
            if (start > end) {
                let temp = start;
                start = end;
                end = temp;
            }
            for (let i = 0; i < dateNumber.length; i++) {
                let date = parseInt(dateNumber[i].parentElement.getAttribute('data-num'));
                if (date >= start && date <= end) {
                    if (dateNumber[i].parentElement.classList.contains("highlight-range")) {
                        return false;
                    }
                }
            }
            return true;
        },
        HighlightSelectedRange: function (flag) {
            if (startDat == null && endDat == null) {
                return;
            }
            if (startDat && endDat == null) {
                if (flag)
                    startDat.parentElement.classList.add("highlight-selected");
                else
                    startDat.parentElement.classList.remove("highlight-selected");
                return;
            }
            let dateNumber = document.querySelectorAll(".calendar .dateNumber");
            let start = parseInt(startDat.parentElement.getAttribute('data-num'));
            let end = parseInt(endDat.parentElement.getAttribute('data-num'));
            if (start > end) {
                let temp = start;
                start = end;
                end = temp;
            }
            for (let i = 0; i < dateNumber.length; i++) {
                let date = parseInt(dateNumber[i].textContent);
                if (date >= start && date <= end) {
                    if (flag)
                        dateNumber[i].parentElement.classList.add("highlight-selected");
                    else
                        dateNumber[i].parentElement.classList.remove("highlight-selected");
                }
            }
        },
        GetStartEndDate: function () {
            const sdatt = new Date(startDat.parentElement.getAttribute('data-num') + "-" + (calendar.getMonth() + 1) + "-" + calendar.getFullYear());
            const edatt = new Date(endDat.parentElement.getAttribute('data-num') + "-" + (calendar.getMonth() + 1) + "-" + calendar.getFullYear());
            return { start: sdatt, end: edatt };
        },
        attachEvents: function () {
            let prevBtn = document.querySelector(".calendar .calendar-prev a");
            let nextBtn = document.querySelector(".calendar .calendar-next a");
            let todayDate = document.querySelector(".calendar .calendar-today-date");
            let dateNumber = document.querySelectorAll(".calendar .dateNumber");
            prevBtn.addEventListener(
                "click",
                calendarControl.navigateToPreviousMonth
            );
            nextBtn.addEventListener("click", calendarControl.navigateToNextMonth);
            todayDate.addEventListener(
                "click",
                calendarControl.navigateToCurrentMonth
            );
            for (var i = 0; i < dateNumber.length; i++) {
                dateNumber[i].addEventListener(
                    "click",
                    calendarControl.selectDate,
                    false
                );
            }
        },
        highlightToday: function () {
            let currentMonth = calendarControl.localDate.getMonth() + 1;
            let changedMonth = calendar.getMonth() + 1;
            let currentYear = calendarControl.localDate.getFullYear();
            let changedYear = calendar.getFullYear();
            if (
                currentYear === changedYear &&
                currentMonth === changedMonth &&
                document.querySelectorAll(".number-item")
            ) {
                document
                    .querySelectorAll(".number-item")
                [calendar.getDate() - 1].classList.add("calendar-today");
            }
        },
        plotPrevMonthDates: function (dates) {
            dates.reverse();
            for (let i = 0; i < dates.length; i++) {
                if (document.querySelectorAll(".prev-dates")) {
                    document.querySelectorAll(".prev-dates")[i].textContent = dates[i];
                }
            }
        },
        plotNextMonthDates: function () {
            let childElemCount = document.querySelector('.calendar-body').childElementCount;
            //7 lines
            if (childElemCount > 42) {
                let diff = 49 - childElemCount;
                calendarControl.loopThroughNextDays(diff);
            }

            //6 lines
            if (childElemCount > 35 && childElemCount <= 42) {
                let diff = 42 - childElemCount;
                calendarControl.loopThroughNextDays(42 - childElemCount);
            }

        },
        loopThroughNextDays: function (count) {
            if (count > 0) {
                for (let i = 1; i <= count; i++) {
                    document.querySelector('.calendar-body').innerHTML += `<div class="next-dates">${i}</div>`;
                }
            }
        },
        attachEventsOnNextPrev: function () {
            calendarControl.plotDates();
            calendarControl.attachEvents();
        },
        init: function () {
            calendarControl.plotSelectors();
            calendarControl.plotDates();
            calendarControl.attachEvents();
        }
    };
    return calendarControl;
}
