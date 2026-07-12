import json
import re

nodes = []
edges = []
hyperedges = []

def make_id(filepath, entity):
    # stem is the full repo-relative path with the extension dropped, every path segment kept and joined with _
    # (each segment lowercased with non-alphanumeric chars replaced by _)
    filepath_no_ext = filepath.rsplit('.', 1)[0]
    # normalize path separators
    filepath_no_ext = filepath_no_ext.replace('\\', '/')
    stem_parts = [re.sub(r'[^a-z0-9_]', '_', part.lower()) for part in filepath_no_ext.split('/')]
    stem = '_'.join(stem_parts)
    entity_norm = re.sub(r'[^a-z0-9_]', '_', entity.lower())
    return f"{stem}_{entity_norm}"

def add_node(filepath, entity, label, file_type):
    node_id = make_id(filepath, entity)
    nodes.append({
        "id": node_id,
        "label": label,
        "file_type": file_type,
        "source_file": filepath,
        "source_location": None,
        "source_url": None,
        "captured_at": None,
        "author": None,
        "contributor": None
    })
    return node_id

def add_edge(source, target, relation, confidence, confidence_score, filepath):
    edges.append({
        "source": source,
        "target": target,
        "relation": relation,
        "confidence": confidence,
        "confidence_score": confidence_score,
        "source_file": filepath,
        "source_location": None,
        "weight": 1.0
    })

def add_hyperedge(id_str, label, node_ids, relation, confidence, confidence_score, filepath):
    hyperedges.append({
        "id": id_str,
        "label": label,
        "nodes": node_ids,
        "relation": relation,
        "confidence": confidence,
        "confidence_score": confidence_score,
        "source_file": filepath
    })


# file 11
f11 = r"D:\CRM\doc\plan\11_get_user_reference.md"
rel_11 = "doc/plan/11_get_user_reference.md"
n11 = add_node(rel_11, "get_user_references", "Get User References", "concept")
n11_api = add_node(rel_11, "api_get_user_references", "GET /api/v1/user-reference/get-all", "concept")
n11_ctrl = add_node(rel_11, "UserReferenceController", "UserReferenceController", "concept")
n11_query = add_node(rel_11, "GetUserReference", "GetUserReference", "concept")
n11_repo_i = add_node(rel_11, "IUserReferenceRepository", "IUserReferenceRepository", "concept")
add_edge(n11, n11_api, "implements", "EXTRACTED", 1.0, f11)
add_edge(n11_ctrl, n11_api, "implements", "EXTRACTED", 1.0, f11)
add_edge(n11_ctrl, n11_query, "references", "EXTRACTED", 1.0, f11)
add_edge(n11_query, n11_repo_i, "references", "EXTRACTED", 1.0, f11)

# file 12
f12 = r"D:\CRM\doc\plan\12_list_notifications.md"
rel_12 = "doc/plan/12_list_notifications.md"
n12 = add_node(rel_12, "list_notifications", "List Notifications", "concept")
n12_api = add_node(rel_12, "api_list_notifications", "GET /api/v1/notification/list", "concept")
n12_ctrl = add_node(rel_12, "NotifcationController", "NotifcationController", "concept")
n12_query = add_node(rel_12, "ListNoitifiaction", "ListNoitifiaction", "concept")
n12_repo_i = add_node(rel_12, "INotificationRepository", "INotificationRepository", "concept")
add_edge(n12, n12_api, "implements", "EXTRACTED", 1.0, f12)
add_edge(n12_ctrl, n12_api, "implements", "EXTRACTED", 1.0, f12)
add_edge(n12_ctrl, n12_query, "references", "EXTRACTED", 1.0, f12)
add_edge(n12_query, n12_repo_i, "references", "EXTRACTED", 1.0, f12)

# file 13
f13 = r"D:\CRM\doc\plan\13_background_jobs.md"
rel_13 = "doc/plan/13_background_jobs.md"
n13 = add_node(rel_13, "background_jobs", "Background Jobs", "concept")
n13_srv_i = add_node(rel_13, "IHangFireService", "IHangFireService", "concept")
n13_reg_i = add_node(rel_13, "IRecurringJobRegistrar", "IRecurringJobRegistrar", "concept")
n13_job1 = add_node(rel_13, "test_hourly", "test-hourly", "concept")
n13_job2 = add_node(rel_13, "reminder_3_days", "reminder-loan-repayment-3-days", "concept")
add_edge(n13, n13_srv_i, "references", "EXTRACTED", 1.0, f13)
add_edge(n13, n13_reg_i, "references", "EXTRACTED", 1.0, f13)
add_edge(n13, n13_job1, "implements", "EXTRACTED", 1.0, f13)
add_edge(n13, n13_job2, "implements", "EXTRACTED", 1.0, f13)

# file 14
f14 = r"D:\CRM\doc\plan\14_core_payment_engine.md"
rel_14 = "doc/plan/14_core_payment_engine.md"
n14 = add_node(rel_14, "core_payment_engine", "Core Payment Engine", "concept")
n14_ctrl = add_node(rel_14, "PaymentController", "PaymentController", "concept")
n14_cmd = add_node(rel_14, "PayLoanCommand", "PayLoanCommand", "concept")
n14_entity = add_node(rel_14, "LoanTransaction", "LoanTransaction", "concept")
add_edge(n14, n14_ctrl, "implements", "EXTRACTED", 1.0, f14)
add_edge(n14_ctrl, n14_cmd, "references", "EXTRACTED", 1.0, f14)
add_edge(n14_cmd, n14_entity, "references", "EXTRACTED", 1.0, f14)

# file 15
f15 = r"D:\CRM\doc\plan\15_overdue_processing.md"
rel_15 = "doc/plan/15_overdue_processing.md"
n15 = add_node(rel_15, "overdue_processing", "Overdue Processing", "concept")
n15_srv_i = add_node(rel_15, "IPenaltyCalculationService", "IPenaltyCalculationService", "concept")
n15_job = add_node(rel_15, "DailyOverdueProcessorJob", "DailyOverdueProcessorJob", "concept")
add_edge(n15, n15_srv_i, "implements", "EXTRACTED", 1.0, f15)
add_edge(n15, n15_job, "implements", "EXTRACTED", 1.0, f15)

# file 16
f16 = r"D:\CRM\doc\plan\16_payment_gateway.md"
rel_16 = "doc/plan/16_payment_gateway.md"
n16 = add_node(rel_16, "payment_gateway", "Payment Gateway", "concept")
n16_api = add_node(rel_16, "api_create_url", "POST /api/v1/payment/create-url", "concept")
n16_srv = add_node(rel_16, "IVNPayService", "IVNPayService", "concept")
n16_ctrl = add_node(rel_16, "PaymentController", "PaymentController", "concept")
add_edge(n16, n16_api, "implements", "EXTRACTED", 1.0, f16)
add_edge(n16_ctrl, n16_api, "implements", "EXTRACTED", 1.0, f16)
add_edge(n16_ctrl, n16_srv, "references", "EXTRACTED", 1.0, f16)

# file 17
f17 = r"D:\CRM\doc\plan\17_disbursement_documents.md"
rel_17 = "doc/plan/17_disbursement_documents.md"
n17 = add_node(rel_17, "disbursement_documents", "Disbursement & Documents", "concept")
n17_api = add_node(rel_17, "api_disburse", "POST /api/v1/loan/{id}/disburse", "concept")
n17_ctrl = add_node(rel_17, "LoanController", "LoanController", "concept")
n17_cmd = add_node(rel_17, "DisburseLoanCommand", "DisburseLoanCommand", "concept")
add_edge(n17, n17_api, "implements", "EXTRACTED", 1.0, f17)
add_edge(n17_ctrl, n17_api, "implements", "EXTRACTED", 1.0, f17)
add_edge(n17_ctrl, n17_cmd, "references", "EXTRACTED", 1.0, f17)

# file 18
f18 = r"D:\CRM\doc\plan\18_collection_analytics.md"
rel_18 = "doc/plan/18_collection_analytics.md"
n18 = add_node(rel_18, "collection_analytics", "Collection & Analytics", "concept")
n18_entity = add_node(rel_18, "CollectionTask", "CollectionTask", "concept")
n18_query = add_node(rel_18, "GetAdminDashboardSummaryQuery", "GetAdminDashboardSummaryQuery", "concept")
add_edge(n18, n18_entity, "references", "EXTRACTED", 1.0, f18)
add_edge(n18, n18_query, "references", "EXTRACTED", 1.0, f18)

# file 1
f1 = r"D:\CRM\doc\plan\1_register.md"
rel_1 = "doc/plan/1_register.md"
n1 = add_node(rel_1, "user_registration", "User Registration", "concept")
n1_api = add_node(rel_1, "api_register", "POST /api/v1/auth/register", "concept")
n1_ctrl = add_node(rel_1, "AuthController", "AuthController", "concept")
n1_cmd = add_node(rel_1, "RegisterCommand", "RegisterCommand", "concept")
add_edge(n1, n1_api, "implements", "EXTRACTED", 1.0, f1)
add_edge(n1_ctrl, n1_api, "implements", "EXTRACTED", 1.0, f1)
add_edge(n1_ctrl, n1_cmd, "references", "EXTRACTED", 1.0, f1)

# file 2
f2 = r"D:\CRM\doc\plan\2_login.md"
rel_2 = "doc/plan/2_login.md"
n2 = add_node(rel_2, "user_login", "User Login", "concept")
n2_api = add_node(rel_2, "api_login", "POST /api/v1/auth/login", "concept")
n2_ctrl = add_node(rel_2, "AuthController", "AuthController", "concept")
n2_cmd = add_node(rel_2, "LoginQuery", "LoginQuery", "concept")
add_edge(n2, n2_api, "implements", "EXTRACTED", 1.0, f2)
add_edge(n2_ctrl, n2_api, "implements", "EXTRACTED", 1.0, f2)
add_edge(n2_ctrl, n2_cmd, "references", "EXTRACTED", 1.0, f2)
# semantics
add_edge(n1_ctrl, n2_ctrl, "semantically_similar_to", "INFERRED", 0.95, f2)

# file 3
f3 = r"D:\CRM\doc\plan\3_get_user_info.md"
rel_3 = "doc/plan/3_get_user_info.md"
n3 = add_node(rel_3, "get_user_info", "Get User Info", "concept")
n3_api = add_node(rel_3, "api_account_info", "GET /api/v1/account/info", "concept")
n3_ctrl = add_node(rel_3, "AccountController", "AccountController", "concept")
n3_cmd = add_node(rel_3, "GetUserInfoQuery", "GetUserInfoQuery", "concept")
add_edge(n3, n3_api, "implements", "EXTRACTED", 1.0, f3)
add_edge(n3_ctrl, n3_api, "implements", "EXTRACTED", 1.0, f3)
add_edge(n3_ctrl, n3_cmd, "references", "EXTRACTED", 1.0, f3)

# file 4
f4 = r"D:\CRM\doc\plan\4_create_loan.md"
rel_4 = "doc/plan/4_create_loan.md"
n4 = add_node(rel_4, "create_loan", "Create Loan", "concept")
n4_api = add_node(rel_4, "api_create_loan", "POST /api/v1/loan/create", "concept")
n4_ctrl = add_node(rel_4, "LoanController", "LoanController", "concept")
n4_cmd = add_node(rel_4, "CreateLoanCommand", "CreateLoanCommand", "concept")
add_edge(n4, n4_api, "implements", "EXTRACTED", 1.0, f4)
add_edge(n4_ctrl, n4_api, "implements", "EXTRACTED", 1.0, f4)
add_edge(n4_ctrl, n4_cmd, "references", "EXTRACTED", 1.0, f4)

# file 5
f5 = r"D:\CRM\doc\plan\5_review_loan.md"
rel_5 = "doc/plan/5_review_loan.md"
n5 = add_node(rel_5, "review_loan", "Review Loan", "concept")
n5_api = add_node(rel_5, "api_review_loan", "POST /api/v1/loan/review", "concept")
n5_ctrl = add_node(rel_5, "LoanController", "LoanController", "concept")
n5_cmd = add_node(rel_5, "ReviewLoanCommand", "ReviewLoanCommand", "concept")
add_edge(n5, n5_api, "implements", "EXTRACTED", 1.0, f5)
add_edge(n5_ctrl, n5_api, "implements", "EXTRACTED", 1.0, f5)
add_edge(n5_ctrl, n5_cmd, "references", "EXTRACTED", 1.0, f5)

# file 6
f6 = r"D:\CRM\doc\plan\6_get_loan_info.md"
rel_6 = "doc/plan/6_get_loan_info.md"
n6 = add_node(rel_6, "get_loan_info", "Get Loan Info", "concept")
n6_api = add_node(rel_6, "api_loan_info", "GET /api/v1/loan/info", "concept")
n6_ctrl = add_node(rel_6, "LoanController", "LoanController", "concept")
n6_cmd = add_node(rel_6, "GetLoanInfo", "GetLoanInfo", "concept")
add_edge(n6, n6_api, "implements", "EXTRACTED", 1.0, f6)
add_edge(n6_ctrl, n6_api, "implements", "EXTRACTED", 1.0, f6)
add_edge(n6_ctrl, n6_cmd, "references", "EXTRACTED", 1.0, f6)

# file 7
f7 = r"D:\CRM\doc\plan\7_get_all_loans.md"
rel_7 = "doc/plan/7_get_all_loans.md"
n7 = add_node(rel_7, "get_all_loans", "Get All Loans", "concept")
n7_api = add_node(rel_7, "api_all_admin", "GET /api/v1/loan/all-admin", "concept")
n7_ctrl = add_node(rel_7, "LoanController", "LoanController", "concept")
n7_cmd = add_node(rel_7, "GetAllLoan", "GetAllLoan", "concept")
add_edge(n7, n7_api, "implements", "EXTRACTED", 1.0, f7)
add_edge(n7_ctrl, n7_api, "implements", "EXTRACTED", 1.0, f7)
add_edge(n7_ctrl, n7_cmd, "references", "EXTRACTED", 1.0, f7)

# file 8
f8 = r"D:\CRM\doc\plan\8_get_user_loans.md"
rel_8 = "doc/plan/8_get_user_loans.md"
n8 = add_node(rel_8, "get_user_loans", "Get User Loans", "concept")
n8_api = add_node(rel_8, "api_all_user", "GET /api/v1/loan/all-user", "concept")
n8_ctrl = add_node(rel_8, "LoanController", "LoanController", "concept")
n8_cmd = add_node(rel_8, "GetAllUserLoan", "GetAllUserLoan", "concept")
add_edge(n8, n8_api, "implements", "EXTRACTED", 1.0, f8)
add_edge(n8_ctrl, n8_api, "implements", "EXTRACTED", 1.0, f8)
add_edge(n8_ctrl, n8_cmd, "references", "EXTRACTED", 1.0, f8)

# file 9
f9 = r"D:\CRM\doc\plan\9_get_loan_repayment_date.md"
rel_9 = "doc/plan/9_get_loan_repayment_date.md"
n9 = add_node(rel_9, "get_loan_repayment_date", "Get Loan Repayment Date", "concept")
n9_api = add_node(rel_9, "api_loan_repayment", "GET /api/v1/loan/repayment", "concept")
n9_ctrl = add_node(rel_9, "LoanController", "LoanController", "concept")
n9_cmd = add_node(rel_9, "GetLoanRepaymentDate", "GetLoanRepaymentDate", "concept")
add_edge(n9, n9_api, "implements", "EXTRACTED", 1.0, f9)
add_edge(n9_ctrl, n9_api, "implements", "EXTRACTED", 1.0, f9)
add_edge(n9_ctrl, n9_cmd, "references", "EXTRACTED", 1.0, f9)


# Grouping all LoanControllers
loan_controllers = [n4_ctrl, n5_ctrl, n6_ctrl, n7_ctrl, n8_ctrl, n9_ctrl, n17_ctrl]
add_hyperedge("doc_plan_loan_controllers", "Loan Controllers Group", loan_controllers, "participate_in", "INFERRED", 0.95, f17)

# Ensure correct file paths are used verbatim
def update_filepaths(edges, hyperedges):
    # Already using f1..f18 which are verbatim paths
    pass

with open(r'D:\CRM\graphify-out\.graphify_chunk_02.json', 'w', encoding='utf-8') as f:
    json.dump({
        "nodes": nodes,
        "edges": edges,
        "hyperedges": hyperedges,
        "input_tokens": 0,
        "output_tokens": 0
    }, f, ensure_ascii=False)

print("Done")
