namespace FSharpClassLibrary.Models

open System

/// Name of the course, hours, and points earned. Uses semester units.
type Course = {
    Name : string
    SemesterHours : float
    SemesterPointsEarned : float
    }

/// School and list of courses taken.
type ReportCard = {
    School : string
    Courses : Course array
    }

/// Term (as approximate month and year) along with report cards for each institution
/// where I took classes during that term.
type TermReportCards = {
    Term : DateTime
    TermName : string
    ReportCards : ReportCard array
    }

